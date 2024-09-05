using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinding : MonoBehaviour
{
    GridMap gridNodes;
    List<Node> openList; // A list to store nodes that have not been checked
    List<Node> closedList; // A list to store nodes that have been checked

    void Start()
    {
        // Initialize the variables
        gridNodes = GetComponent<GridMap>();
        openList = new List<Node>();
        closedList = new List<Node>();
    }

    /// <summary>
    /// Get the node with the lowest F cost in the open list
    /// </summary>
    /// <returns>The node with the lowest F cost</returns>
    Node GetLowestFCostNode()
    {
        // Do nothing if the list is empty
        if (openList.Count == 0) return null;

        // Get the first node in the list
        Node node = openList[0];

        // Loop through each node in the list
        for (int i = 1; i < openList.Count; ++i)
        {
            // Compare their F costs, keep the node with the lower cost
            if (node.FCost > openList[i].FCost) node = openList[i];
        }

        // Return the node with the lowest F cost
        return node;
    }

    /// <summary>
    /// Retraces the path that was computed
    /// </summary>
    /// <param name="startNode">The start node</param>
    /// <param name="endNode">The end node</param>
    /// <returns></returns>
    Vector3[] RetracePath(Node startNode, Node endNode)
    {
        // Create a list to store the path
        List<Vector3> path = new List<Vector3>();
        // Start from the back
        Node currNode = endNode;

        // Continue adding nodes until the start node has been reached
        while (currNode != startNode)
        {
            path.Add(currNode.Position);
            currNode = currNode.Parent;
        }

        // As the list is currently starting from the back,
        // It has to be reversed first to start from the startNode
        path.Reverse();
        // Return the path
        return path.ToArray();
    }

    /// <summary>
    /// Compute the shortest path from 1 point to another
    /// </summary>
    /// <param name="startPos">The start position</param>
    /// <param name="endPos">The end position</param>
    /// <param name="colliderSize">The size of the object</param>
    /// <returns>The shortest path</returns>
    public Vector3[] AStarPathfinding(Vector3 startPos, Vector3 endPos, float colliderSize = 0f)
    {
        // Get the start and end nodes
        Node startNode = gridNodes.GetClosestNode(startPos, true);
        Node endNode = gridNodes.GetClosestNode(endPos, true);

        // Log an error if no start or end node can be found
        if (startNode == null || endNode == null)
        {
            Debug.LogError("Start or end nodes could not be found.");
            return null;
        }

        // Reset every node that has been accessed previously
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

        // Clear the lists
        openList.Clear();
        closedList.Clear();

        // Set the start node's costs
        startNode.SetAsStartNode(gridNodes.GetDistanceCost(startNode, endNode));

        // Add it to the open list
        openList.Add(startNode);

        // Loop through the open list
        while (openList.Count > 0)
        {
            // Get the node with the lowest F cost
            Node currNode = GetLowestFCostNode();

            // If the current node is the end node, return the path to it
            if (currNode == endNode) return RetracePath(startNode, endNode);

            // As the current node is already being checked,
            // We can remove it from the open list and add it to the closed list
            openList.Remove(currNode);
            closedList.Add(currNode);

            // Loop through all of thhe node's neighbors
            foreach (Node neighbor in gridNodes.GetNeighbors(currNode, colliderSize))
            {
                // Ignore any that have already been checked or that are not travellable
                if (neighbor.NodeType != NodeType.FLOOR || closedList.Contains(neighbor)) continue;

                // Get the movement cost of the node to its neighbor
                float movementCost = currNode.GCost + gridNodes.GetDistanceCost(currNode, neighbor);

                // Add the neighbor to the open list if the movement cost is smaller than the neighbor's G cost
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

        // Loop through every node in the closed list
        foreach (Node node in closedList)
        {
            // Get the distance of the current node from the end node
            float distance = gridNodes.GetDistanceCost(node, endNode);

            // Store the node and distance if the current node is nearer to the end node
            // Compared to the previous closest node
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        // Retrace the path to the closest node
        if (closestNode != null) return RetracePath(startNode, closestNode);

        // Return null if no path was found
        return null;
    }

    void OnDestroy()
    {
        // Clear the lists
        openList.Clear();
        closedList.Clear();
    }
}