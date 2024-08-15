using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    GridNodes gridNodes;
    List<Node> openList;
    List<Node> closedList;

    void Start()
    {
        gridNodes = GetComponent<GridNodes>();
        openList = new List<Node>();
        closedList = new List<Node>();
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
        Node startNode = gridNodes.GetClosestNode(startPos);
        Node endNode = gridNodes.GetClosestNode(endPos);

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

        startNode.SetAsStartNode(gridNodes.GetDistanceCost(startNode, endNode));

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currNode = GetLowestFCostNode();

            if (currNode == endNode) return RetracePath(startNode, endNode);

            openList.Remove(currNode);
            closedList.Add(currNode);

            foreach (Node neighbor in gridNodes.GetNeighbors(currNode, colliderSize))
            {
                if (neighbor.NodeType != NodeType.FLOOR || closedList.Contains(neighbor)) continue;

                float movementCost = currNode.GCost + gridNodes.GetDistanceCost(currNode, neighbor);

                if (movementCost < neighbor.GCost)
                {
                    neighbor.GCost = movementCost;
                    neighbor.HCost = gridNodes.GetDistanceCost(neighbor, endNode);
                    neighbor.Parent = currNode;

                    if (!openList.Contains(neighbor)) openList.Add(neighbor);
                }
            }
        }

        // If the loop exits without finding a path, find the closest node to the endNode
        Node closestNode = null;
        float closestDistance = float.MaxValue;

        foreach (Node node in closedList)
        {
            float distance = gridNodes.GetDistanceCost(node, endNode);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        if (closestNode != null) return RetracePath(startNode, closestNode);

        return null;
    }

    void OnDestroy()
    {
        openList.Clear();
        closedList.Clear();
    }
}