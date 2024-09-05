using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridMap : MonoBehaviour
{
    // The grid containing the tilemaps
    [SerializeField] Grid grid = null;
    // A dictionary of all the nodes in the map
    Dictionary<Vector3, Node> allNodes = new Dictionary<Vector3, Node>();
    // A dictionary of all the spatially partitioned cells
    Dictionary<Vector3Int, Dictionary<Vector3, Node>> partitionedNodes = new Dictionary<Vector3Int, Dictionary<Vector3, Node>>();

    // The size of each node
    [SerializeField] float nodeSize = 0.25f;
    // The minimum size the nodes could be
    float minNodeSize = 0.1f;

    // The size of each spatially partitioned cell
    [SerializeField] int cellSize = 1;
    // The minimum size the cells could be
    int minCellSize = 1;
    // The opacity of the cells
    [SerializeField] float cellOpacity = 1f;

    // A bool to toggle the display of the nodes
    [SerializeField] bool showNodes = true;
    // The layers that should be treated as obstacles
    [SerializeField] LayerMask obstacleLayer;

    // The colors that floor and obstacle nodes should have
    [SerializeField] Color floorNodeColor = Color.green;
    [SerializeField] Color obstacleNodeColor = Color.red;

    // The bounds of the entire map
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
    /// Initialize the nodes in the map
    /// </summary>
    public void InitializeNodes(Vector2 minBounds, Vector2 maxBounds)
    {
        // Do nothing if the node or cell size is too small
        if (nodeSize < minNodeSize || cellSize < minCellSize) return;

        // Clear all existing nodes
        partitionedNodes.Clear();
        allNodes.Clear();

        // Get bounds
        minX = minBounds.x;
        minY = minBounds.y;
        maxX = maxBounds.x;
        maxY = maxBounds.y;

        // Find the number of nodes there should be in the x and y axis
        int numNodesX = Mathf.RoundToInt((maxBounds.x - minBounds.x) / nodeSize);
        int numNodesY = Mathf.RoundToInt((maxBounds.y - minBounds.y) / nodeSize);

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

    void AddNodeToPartition(Node node)
    {
        Vector3Int pos = GetPartitionCell(node.Position);
        if (!partitionedNodes.ContainsKey(pos)) partitionedNodes[pos] = new Dictionary<Vector3, Node>();
        partitionedNodes[pos][node.Position] = node;
    }

    Vector3Int GetPartitionCell(Vector3 pos)
    {
        return new Vector3Int(Mathf.FloorToInt(pos.x / cellSize), Mathf.FloorToInt(pos.y / cellSize), 0);
    }

    public NodeType GetNodeType(Vector3 pos, float colliderSize = 0f)
    {
        float size = nodeSize > colliderSize ? nodeSize : colliderSize;
        Vector2 pointA = new Vector2(pos.x - size / 2, pos.y - size / 2);
        Vector2 pointB = new Vector2(pos.x + size / 2, pos.y + size / 2);

        Collider2D[] colliders = Physics2D.OverlapAreaAll(pointA, pointB);
        foreach (var collider in colliders)
        {
            int layer = collider.gameObject.layer;
            if ((obstacleLayer & (1 << layer)) != 0) return NodeType.OBSTACLE;
        }
        return NodeType.FLOOR;
    }

    public Vector3 GetEmptyPosition(float size)
    {
        List<Vector3> emptyPos = new List<Vector3>();
        Vector3Int playerCellPos = GetPartitionCell(GameplayManager.instance.Player.transform.position);
        int maxRange = Mathf.CeilToInt(7 / cellSize);
        int minRange = Mathf.CeilToInt(2 / cellSize);

        foreach (KeyValuePair<Vector3Int, Dictionary<Vector3, Node>> cell in partitionedNodes)
        {
            float dist = Vector3Int.Distance(playerCellPos, cell.Key);
            if (minRange > dist && dist > maxRange) continue;

            foreach (KeyValuePair<Vector3, Node> node in cell.Value)
            {
                if (node.Value.NodeType == NodeType.FLOOR)
                {
                    emptyPos.Add(node.Key);
                    break;
                }
            }
        }
        if (emptyPos.Count == 0) Debug.LogError("No empty positions could be found!");

        int randomIndex = Random.Range(0, emptyPos.Count);
        return emptyPos[randomIndex];
    }

    public Node GetClosestNode(Vector3 pos, bool checkObstruction)
    {
        Node node = null;
        float minDist = float.MaxValue;

        foreach (var otherNode in allNodes.Values)
        {
            float dist = Vector2.Distance(pos, otherNode.Position);

            if (dist < minDist)
            {
                if (checkObstruction && otherNode.NodeType == NodeType.OBSTACLE) continue;
                node = otherNode;
                minDist = dist;
            }
        }

        return node;
    }

    public List<Node> GetNeighbors(Node node, float colliderSize = 0f)
    {
        List<Node> neighbors = new List<Node>();

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

        foreach (Vector3 direction in directions)
        {
            Vector3 neighbor = node.Position + direction;
            if (neighbor.x > maxX) neighbor.x = maxX;
            if (neighbor.y > maxY) neighbor.y = maxY;

            Vector3Int cellPos = GetPartitionCell(neighbor);

            if (!partitionedNodes.ContainsKey(cellPos)) continue;
            if (!partitionedNodes[cellPos].ContainsKey(neighbor)) continue;

            if (GetNodeType(neighbor, colliderSize) == NodeType.FLOOR) neighbors.Add(partitionedNodes[cellPos][neighbor]);
        }

        return neighbors;
    }

    public float GetDistanceCost(Node nodeA, Node nodeB)
    {
        float xDist = Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
        float yDist = Mathf.Abs(nodeA.Position.y - nodeB.Position.y);
        float dist = diagonalMovementCost * Mathf.Min(xDist, yDist) + straightMovementCost * Mathf.Abs(xDist - yDist);
        return dist;
    }

    public bool IsPathClear(Vector2 posA, Vector2 posB)
    {
        if (Physics2D.Linecast(posA, posB, obstacleLayer)) return true;
        return false;
    }

    void OnDestroy()
    {
        allNodes.Clear();

        foreach (var dict in partitionedNodes.Values)
            dict.Clear();

        partitionedNodes.Clear();
    }
}