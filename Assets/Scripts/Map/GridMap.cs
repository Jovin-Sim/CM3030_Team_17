using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMap : MonoBehaviour
{
    [Tooltip("The grid containing the tilemaps")]
    [SerializeField] Grid grid = null;

    // A dictionary of all the nodes in the map
    Dictionary<Vector3, Node> allNodes = new Dictionary<Vector3, Node>();
    // A dictionary of all the spatially partitioned cells
    Dictionary<Vector3Int, Dictionary<Vector3, Node>> partitionedNodes = new Dictionary<Vector3Int, Dictionary<Vector3, Node>>();

    [Tooltip("The size of each node")]
    [SerializeField] float nodeSize = 0.25f;
    // The minimum size the nodes could be
    float minNodeSize = 0.1f;

    [Tooltip("The size of each spatially partitioned cell")]
    [SerializeField] int cellSize = 1;
    // The minimum size the cells could be
    int minCellSize = 1;
    [Tooltip("The opacity of the cells")]
    [SerializeField] float cellOpacity = 1f;

    [Tooltip("A bool to toggle the display of the nodes")]
    [SerializeField] bool showNodes = true;

    [Tooltip("The layers that should be treated as obstacles")]
    [SerializeField] LayerMask obstacleLayer;

    [Tooltip("The colors that floor and obstacle nodes should have")]
    [SerializeField] Color floorNodeColor = Color.green;
    [SerializeField] Color obstacleNodeColor = Color.red;

    [Tooltip("The bounds of the entire map")]
    public float minX, minY, maxX, maxY;

    // The cost of straight and diagonal movement
    const int straightMovementCost = 10;
    const int diagonalMovementCost = 14;

    #region Getters & Setters
    public float NodeSize
    {
        get { return nodeSize; }
        set { nodeSize = value; }
    }
    public Dictionary<Vector3, Node> AllNodes { get { return allNodes; } }
    public LayerMask ObstacleLayer { get { return obstacleLayer; } }
    #endregion

    void Awake()
    {
        // Retrieve the grid from the scene
        grid = FindObjectOfType<Grid>();
    }

    void OnDrawGizmosSelected()
    {
        // Do nothing if there are no nodes or if the nodes should not be displayed
        if (allNodes == null || partitionedNodes == null || !showNodes) return;

        // Draw the nodes
        foreach (var node in allNodes.Values)
        {
            switch (node.NodeType)
            {
                case NodeType.FLOOR:
                    Gizmos.color = floorNodeColor;
                    break;
                case NodeType.OBSTACLE:
                    Gizmos.color = obstacleNodeColor;
                    break;
            }

            Gizmos.DrawCube(node.Position, Vector3.one * 0.1f);
        }

        Gizmos.color = new Color(0, 0, 1, cellOpacity);
        foreach (var cell in partitionedNodes.Keys)
        {
            Vector3 pos = new Vector3(cell.x * cellSize + cellSize / 2.0f, cell.y * cellSize + cellSize / 2.0f, 0);
            Vector3 size = new Vector3(cellSize, cellSize, 1);
            Gizmos.DrawWireCube(pos, size);
        }
    }

    /// <summary>
    /// Retrieve all tilemaps within the scene
    /// </summary>
    /// <returns>A list of the tilemaps</returns>
    List<Tilemap> RetrieveTilemapsFromGrid()
    {
        List<Tilemap> tilemaps = new List<Tilemap>();
        if (grid == null) return tilemaps;

        tilemaps.AddRange(grid.GetComponentsInChildren<Tilemap>());
        return tilemaps;
    }

    /// <summary>
    /// Initialize the nodes in the map
    /// </summary>
    void InitializeNodes()
    {
        // Do nothing if the node or cell size is too small
        if (nodeSize < minNodeSize || cellSize < minCellSize) return;

        // Clear all existing nodes
        partitionedNodes.Clear();
        allNodes.Clear();

        // Retrieve all the tilemaps in the scene
        List<Tilemap> tilemaps = RetrieveTilemapsFromGrid();
        // Do nothing if there are no tilemaps
        if (tilemaps.Count == 0) return;

        // Calculate grid bounds of the entire map
        minX = minY = int.MaxValue;
        maxX = maxY = int.MinValue;
        foreach (Tilemap tilemap in tilemaps)
        {
            BoundsInt bounds = tilemap.cellBounds;
            minX = Mathf.Min(minX, bounds.xMin);
            minY = Mathf.Min(minY, bounds.yMin);
            maxX = Mathf.Max(maxX, bounds.xMax);
            maxY = Mathf.Max(maxY, bounds.yMax);
        }

        // Find the number of nodes there should be in the x and y axis
        int numNodesX = Mathf.RoundToInt((maxX - minX) / nodeSize);
        int numNodesY = Mathf.RoundToInt((maxY - minY) / nodeSize);

        for (int x = 0; x <= numNodesX; ++x)
        {
            for (int y = 0; y <= numNodesY; ++y)
            {
                // Get the position of the current node to be created
                float posX = Mathf.Min(minX + x * nodeSize, maxX);
                float posY = Mathf.Min(minY + y * nodeSize, maxY);

                Vector3 nodePosition = new Vector3(posX, posY, 0);
                // Get the type of the node
                NodeType nodeType = GetNodeType(nodePosition);

                // Create the node if it does not exist
                if (!allNodes.ContainsKey(nodePosition))
                {
                    Node node = new Node(nodePosition, nodeType);
                    allNodes.Add(nodePosition, node);
                    AddNodeToPartition(node);
                }
                // Change the node type if the node already exists and is of a different node type
                else if (allNodes[nodePosition].NodeType < nodeType) allNodes[nodePosition].NodeType = nodeType;
            }
        }
    }

    /// <summary>
    /// Initializes the nodes in the current zone
    /// </summary>
    /// <param name="bounds">The bounds of the map</param>
    public void InitializeNodes(Bounds bounds)
    {
        // Do nothing if the node or cell size is too small
        if (nodeSize < minNodeSize || cellSize < minCellSize) return;

        // Clear all existing nodes
        partitionedNodes.Clear();
        allNodes.Clear();

        // Get bounds
        minX = bounds.min.x;
        minY = bounds.min.y;
        maxX = bounds.max.x;
        maxY = bounds.max.y;

        // Find the number of nodes there should be in the x and y axis
        int numNodesX = Mathf.RoundToInt((maxX - minX) / nodeSize);
        int numNodesY = Mathf.RoundToInt((maxY - minY) / nodeSize);

        for (int x = 1; x < numNodesX; ++x)
        {
            for (int y = 1; y < numNodesY; ++y)
            {
                // Get the position of the current node to be created
                float posX = Mathf.Min(minX + x * nodeSize, maxX);
                float posY = Mathf.Min(minY + y * nodeSize, maxY);

                Vector3 nodePosition = new Vector3(posX, posY, 0);
                // Get the type of the node
                NodeType nodeType = GetNodeType(nodePosition);

                // Create the node if it does not exist
                if (!allNodes.ContainsKey(nodePosition))
                {
                    Node node = new Node(nodePosition, nodeType);
                    allNodes.Add(nodePosition, node);
                    AddNodeToPartition(node);
                }
                // Change the node type if the node already exists and is of a different node type
                else if (allNodes[nodePosition].NodeType < nodeType) allNodes[nodePosition].NodeType = nodeType;
            }
        }
    }

    /// <summary>
    /// Adds a node to a partition cell
    /// </summary>
    /// <param name="node">The node to add</param>
    void AddNodeToPartition(Node node)
    {
        // Get the cell the node belongs to
        Vector3Int pos = GetPartitionCell(node.Position);

        // Check if the cell exists, create it if it does not
        if (!partitionedNodes.ContainsKey(pos)) partitionedNodes[pos] = new Dictionary<Vector3, Node>();

        // Add the node to the cell
        partitionedNodes[pos][node.Position] = node;
    }

    /// <summary>
    /// Gets the cell that a given position resides in
    /// </summary>
    /// <param name="pos">The position</param>
    /// <returns>The Vector3Int coordinate of the cell</returns>
    Vector3Int GetPartitionCell(Vector3 pos)
    {
        // Return the Vector3Int coordinate of the cell that the position resides in
        return new Vector3Int(Mathf.FloorToInt(pos.x / cellSize), Mathf.FloorToInt(pos.y / cellSize), 0);
    }

    /// <summary>
    /// Calculate the type of the current node
    /// </summary>
    /// <param name="pos">The position of the node</param>
    /// <param name="colliderSize">The size of the collider</param>
    /// <returns>The node type</returns>
    public NodeType GetNodeType(Vector3 pos, float colliderSize = 0f)
    {
        // Get the radius of the area that should be checked
        float size = nodeSize > colliderSize ? nodeSize : colliderSize;
        // Get the bounds that should be checked
        Vector2 pointA = new Vector2(pos.x - size / 2, pos.y - size / 2);
        Vector2 pointB = new Vector2(pos.x + size / 2, pos.y + size / 2);

        // Get all colliders that are within the bounds
        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB);
        foreach (var collider in colliders)
        {
            // Check if the node is an obstacle, return obstacle if it is
            int layer = collider.gameObject.layer;
            if ((obstacleLayer & (1 << layer)) != 0) return NodeType.OBSTACLE;
        }
        // Return floor if no obstacles were found
        return NodeType.FLOOR;
    }

    /// <summary>
    /// Get a random empty position in the map that is within an appropriate range from the player
    /// </summary>
    /// <returns>Returns the empty position</returns>
    public Vector3 GetEmptyPosition()
    {
        // Create a list to store the empty positions
        List<Vector3> emptyPos = new List<Vector3>();
        // Get the cell the player resides in
        Vector3Int playerCellPos = GetPartitionCell(GameplayManager.instance.Player.transform.position);
        // Get the range that the empty position should be in
        int maxRange = Mathf.CeilToInt(7 / cellSize);
        int minRange = Mathf.CeilToInt(2 / cellSize);

        // Loop through all cells
        foreach (KeyValuePair<Vector3Int, Dictionary<Vector3, Node>> cell in partitionedNodes)
        {
            // Get the distance of the cell from the player's cell
            float dist = Vector3Int.Distance(playerCellPos, cell.Key);
            // Reject any that are not at an acceptable range from the player cell
            if (minRange > dist && dist > maxRange) continue;

            // Loop through the nodes in the cell
            foreach (KeyValuePair<Vector3, Node> node in cell.Value)
            {
                // Save the first node in the cell in emptyPos
                if (node.Value.NodeType == NodeType.FLOOR)
                {
                    emptyPos.Add(node.Key);
                    break;
                }
            }
        }
        // An empty position was not found
        if (emptyPos.Count == 0)
        {
            Debug.LogError("No empty positions could be found!");

            return Vector3.zero;
        }

        // Return a random position in emptyPos
        int randomIndex = Random.Range(0, emptyPos.Count);
        return emptyPos[randomIndex];
    }

    /// <summary>
    /// Get the closest node to a specified position
    /// </summary>
    /// <param name="pos">The position to check</param>
    /// <param name="checkObstruction">A boolean to not return obstruction nodes</param>
    /// <returns>The closest node</returns>
    public Node GetClosestNode(Vector3 pos, bool checkObstruction)
    {
        // Create a Node object to store the closest node
        Node node = null;
        // And a float to store the distance of the node from the position
        float minDist = float.MaxValue;

        // Loop through every node
        foreach (var otherNode in allNodes.Values)
        {
            // Get the distance of the node from the position
            float dist = Vector2.Distance(pos, otherNode.Position);

            // Check if this node is nearer than the current nearest node
            if (dist < minDist)
            {
                // Check if it is an obstruction and ignore it if checkObstruction is true
                if (checkObstruction && otherNode.NodeType == NodeType.OBSTACLE) continue;

                // Store this node in the Node object alongside its distance
                node = otherNode;
                minDist = dist;
            }
        }

        // Return the closest node
        return node;
    }

    /// <summary>
    /// Get the neighbors of a given node that can be travelled to
    /// </summary>
    /// <param name="node">The node to check</param>
    /// <param name="colliderSize">The size of the travelling object</param>
    /// <returns>The node's neighbors</returns>
    public List<Node> GetNeighbors(Node node, float colliderSize = 0f)
    {
        // Create a list to store the neighbors
        List<Node> neighbors = new List<Node>();

        // Store the directions of the neighbors to check for
        Vector3[] directions = {
            new Vector3(nodeSize, 0, 0),
            new Vector3(-nodeSize, 0, 0),
            new Vector3(0, nodeSize, 0),
            new Vector3(0, -nodeSize, 0),
            new Vector3(nodeSize, nodeSize, 0),
            new Vector3(-nodeSize, nodeSize, 0),
            new Vector3(nodeSize, -nodeSize, 0),
            new Vector3(-nodeSize, -nodeSize, 0)
        };

        // Loop through each direction
        foreach (Vector3 direction in directions)
        {
            // Get the position of the neighbor
            Vector3 neighbor = node.Position + direction;

            // Ignore any that are outside the grid bounds
            if (neighbor.x > maxX) neighbor.x = maxX;
            if (neighbor.y > maxY) neighbor.y = maxY;

            // Get the partition cell of the neighbor
            Vector3Int cellPos = GetPartitionCell(neighbor);

            // Check if partitionedNodes and its cells contain the neighbor
            if (!partitionedNodes.ContainsKey(cellPos)) continue;
            if (!partitionedNodes[cellPos].ContainsKey(neighbor)) continue;

            // Add the neighbor into the list if it iis travellable to
            if (GetNodeType(neighbor, colliderSize) == NodeType.FLOOR) neighbors.Add(partitionedNodes[cellPos][neighbor]);
        }

        // Return the list of neighbors
        return neighbors;
    }

    /// <summary>
    /// Get the cost to travel from a given node to another
    /// </summary>
    /// <param name="nodeA">The start node</param>
    /// <param name="nodeB">The end node</param>
    /// <returns>The distance cost</returns>
    public float GetDistanceCost(Node nodeA, Node nodeB)
    {
        // Get the x and y distance of the nodes
        float xDist = Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
        float yDist = Mathf.Abs(nodeA.Position.y - nodeB.Position.y);

        // Get their overall distance from each other, accounting for diagonal travel
        float dist = diagonalMovementCost * Mathf.Min(xDist, yDist) + straightMovementCost * Mathf.Abs(xDist - yDist);

        // Return the distance
        return dist;
    }

    /// <summary>
    /// Check if a given path is free of obstacles
    /// </summary>
    /// <param name="posA">The start point</param>
    /// <param name="posB">The end point</param>
    /// <returns>A boolean for whether the path is clear</returns>
    public bool IsPathClear(Vector2 posA, Vector2 posB)
    {
        // Do a line cast to check if the path is free of obstacles
        if (Physics2D.Linecast(posA, posB, obstacleLayer)) return true;
        return false;
    }

    void OnDestroy()
    {
        // Clear the dictionary of nodes
        allNodes.Clear();

        // Loop through each cell and clear its contents
        foreach (var dict in partitionedNodes.Values)
            dict.Clear();

        // Clear the dictionary of cells
        partitionedNodes.Clear();
    }
}