// Pathfinding.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Pathfinding : MonoBehaviour
{
    Dictionary<Vector3, Node> allNodes = new Dictionary<Vector3, Node>();
    List<Node> openList;
    List<Node> closedList;

    [SerializeField] bool showNodes = true;
    [SerializeField] LayerMask obstacleLayer;
    [SerializeField] Color floorNodeColor = Color.green;
    [SerializeField] Color obstacleNodeColor = Color.red;

    [SerializeField] float nodeSize = 0.25f;
    float minNodeSize = 0.1f;

    int minX, minY, maxX, maxY;

    const int straightMovementCost = 10;
    const int diagonalMovementCost = 14;

    private void Awake()
    {
        openList = new List<Node>();
        closedList = new List<Node>();
    }

    void Start()
    {
        if (nodeSize < minNodeSize) return;

        if (allNodes.Count > 0) return;

        InitializeNodes();
    }

    void OnDrawGizmos()
    {
        if (allNodes == null || !showNodes) return;

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
    }

    void OnValidate()
    {
        if (nodeSize < minNodeSize) return;
        InitializeNodes();
    }

    public Dictionary<Vector3, Node> GetAllNodes()
    {
        return allNodes;
    }

    List<Tilemap> RetrieveTilemapsFromGrid()
    {
        List<Tilemap> tilemaps = new List<Tilemap>();
        tilemaps.AddRange(GetComponentsInChildren<Tilemap>());
        return tilemaps;
    }

    NodeType GetNodeType(Vector3 pos, float colliderSize = 0f)
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
                }
                else if (allNodes[nodePosition].NodeType < nodeType) allNodes[nodePosition].NodeType = nodeType;
            }
        }
    }

    Node GetClosestNode(Vector3 pos)
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
    Node GetLowestFCostNode()
    {
        if (openList.Count == 0) return null;

        Node node = openList[0];

        for (int i = 1; i < openList.Count; ++i)
        {
            if (node.FCost > openList[i].FCost) node = openList[i];
        }

        return node;
    }

    List<Node> GetNeighbors(Node node, float colliderSize = 0f)
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
            if (!allNodes.ContainsKey(neighbor)) continue;

            if (neighbor.x > maxX) neighbor.x = maxX;
            if (neighbor.y > maxY) neighbor.y = maxY;

            if (GetNodeType(allNodes[neighbor].Position, colliderSize) == NodeType.FLOOR) neighbors.Add(allNodes[neighbor]);
        }

        return neighbors;
    }

    float GetDistanceCost(Node nodeA, Node nodeB)
    {
        float xDist = Mathf.Abs(nodeA.Position.x - nodeB.Position.x);
        float yDist = Mathf.Abs(nodeA.Position.y - nodeB.Position.y);
        float dist = diagonalMovementCost * Mathf.Min(xDist, yDist) + straightMovementCost * Mathf.Abs(xDist - yDist);
        return dist;
    }

    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        List<Vector3> path = new List<Vector3>();
        Node currNode = endNode;

        while (currNode != startNode)
        {
            path.Add(currNode.Position);
            currNode = currNode.Parent;
        }

        path.Reverse();
        return path.ToArray();
    }

    public Vector3[] AStarPathfinding(Vector3 startPos, Vector3 endPos, float colliderSize = 0f)
    {
        Node startNode = GetClosestNode(startPos);
        Node endNode = GetClosestNode(endPos);

        if (startNode == null || endNode == null)
        {
            Debug.LogError("Start or end nodes could not be found.");
            return null;
        }

        if (openList.Count > 0)
        {
            foreach (Node node in openList)
            {
                node.ResetNode();
            }
        }
        if (closedList.Count > 0)
        {
            foreach (Node node in closedList)
            {
                node.ResetNode();
            }
        }

        openList.Clear();
        closedList.Clear();

        startNode.SetAsStartNode(GetDistanceCost(startNode, endNode));

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currNode = GetLowestFCostNode();

            if (currNode == endNode) return RetracePath(startNode, endNode);

            openList.Remove(currNode);
            closedList.Add(currNode);

            foreach (Node neighbor in GetNeighbors(currNode, colliderSize))
            {
                if (neighbor.NodeType != NodeType.FLOOR || closedList.Contains(neighbor)) continue;

                float movementCost = currNode.GCost + GetDistanceCost(currNode, neighbor);

                if (movementCost < neighbor.GCost)
                {
                    neighbor.GCost = movementCost;
                    neighbor.HCost = GetDistanceCost(neighbor, endNode);
                    neighbor.Parent = currNode;

                    if (!openList.Contains(neighbor)) openList.Add(neighbor);
                }
            } 
        }
        return null;
    }

    void OnDestroy()
    {
        allNodes.Clear();
        openList.Clear();
        closedList.Clear();
    }
}