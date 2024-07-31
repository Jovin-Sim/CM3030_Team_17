using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GridNodes : MonoBehaviour
{
    [SerializeField] Grid grid;
    Dictionary<Vector3, Node> allNodes = new Dictionary<Vector3, Node>();
    Dictionary<Vector3Int, Dictionary<Vector3, Node>> partitionedNodes = new Dictionary<Vector3Int, Dictionary<Vector3, Node>>();

    [SerializeField] float nodeSize = 0.25f;
    float minNodeSize = 0.1f;

    [SerializeField] int cellSize = 1;
    int minCellSize = 1;
    [SerializeField] float cellOpacity = 1f;

    [SerializeField] bool showNodes = true;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] Color floorNodeColor = Color.green;
    [SerializeField] Color obstacleNodeColor = Color.red;

    int minX, minY, maxX, maxY;

    const int straightMovementCost = 10;
    const int diagonalMovementCost = 14;

    void Awake()
    {
        grid = FindObjectOfType<Grid>();
    }

    void Start()
    {
        if (nodeSize < minNodeSize || cellSize < minCellSize || allNodes.Count > 0) return;
        InitializeNodes();
    }

    void OnDrawGizmos()
    {
        if (allNodes == null || partitionedNodes == null || !showNodes) return;

        // Draw nodes
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

            Gizmos.DrawCube(node.Position, Vector3.one * 0.05f);
        }

        Gizmos.color = new Color(0, 0, 1, cellOpacity);
        foreach (var cell in partitionedNodes.Keys)
        {
            Vector3 pos = new Vector3(cell.x * cellSize + cellSize / 2.0f, cell.y * cellSize + cellSize / 2.0f, 0);
            Vector3 size = new Vector3(cellSize, cellSize, 1);
            Gizmos.DrawWireCube(pos, size);
        }
    }

    void OnValidate()
    {
        if (nodeSize < minNodeSize || cellSize < minCellSize) return;
        InitializeNodes();
    }

    public float NodeSize
    { 
        get { return nodeSize; } 
        set { nodeSize = value; } 
    }

    public Dictionary<Vector3, Node> AllNodes { get { return allNodes; } }

    List<Tilemap> RetrieveTilemapsFromGrid()
    {
        if (grid == null) return null;

        List<Tilemap> tilemaps = new List<Tilemap>();
        tilemaps.AddRange(grid.GetComponentsInChildren<Tilemap>());
        return tilemaps;
    }

    void InitializeNodes()
    {
        List<Tilemap> tilemaps = RetrieveTilemapsFromGrid();
        if (tilemaps.Count == 0) return;

        allNodes.Clear();

        // Calculate grid bounds
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

        int numNodesX = Mathf.RoundToInt((maxX - minX) / nodeSize);
        int numNodesY = Mathf.RoundToInt((maxY - minY) / nodeSize);

        for (int x = 0; x <= numNodesX; ++x)
        {
            for (int y = 0; y <= numNodesY; ++y)
            {
                float posX = Mathf.Min(minX + x * nodeSize, maxX);
                float posY = Mathf.Min(minY + y * nodeSize, maxY);

                Vector3 nodePosition = new Vector3(posX, posY, 0);
                NodeType nodeType = GetNodeType(nodePosition);

                if (!allNodes.ContainsKey(nodePosition))
                {
                    Node node = new Node(nodePosition, nodeType);
                    allNodes.Add(nodePosition, node);
                    AddNodeToPartition(node);
                }
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
        int range = Mathf.CeilToInt(10 / cellSize);

        for (int x = playerCellPos.x - range; x <= playerCellPos.x + range; x += cellSize)
        {
            if (x < minX || x > maxX) continue;
            for (int y = playerCellPos.x - range; y <= playerCellPos.x + range; y += cellSize)
            {
                if (y < minY || y > maxY) continue;

                Vector3Int cellPos = new Vector3Int(x, y, 0);

                if (!partitionedNodes.ContainsKey(cellPos)) continue;

                foreach (Vector3 pos in partitionedNodes[cellPos].Keys)
                {
                    if (GetNodeType(pos, size) == NodeType.FLOOR &&
                        Vector3.Distance(GameplayManager.instance.Player.transform.position, pos) <= 10)
                        emptyPos.Add(pos);
                }
            }
        }

        if (emptyPos.Count == 0) return Vector3.zero;

        int randomIndex = Random.Range(0, emptyPos.Count);
        return emptyPos[randomIndex];
    }

    public Node GetClosestNode(Vector3 pos)
    {
        Node node = null;
        float minDist = float.MaxValue;

        foreach (var otherNode in allNodes.Values)
        {
            float dist = Vector2.Distance(pos, otherNode.Position);

            if (dist < minDist)
            {
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

    void OnDestroy()
    {
        allNodes.Clear();

        foreach (var dict in partitionedNodes.Values)
            dict.Clear();

        partitionedNodes.Clear();
    }
}