using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum NodeType
{
    FLOOR,
    OBSTACLE,
    NONE
}

public class Node
{
    public Vector3 Position { get; set; }
    public NodeType NodeType { get; set; }
    public Node Parent { get; set; }
    public float GCost { get; set; }  // Cost from the start node to this node
    public float HCost { get; set; }  // Cost from the end node
    public float FCost => GCost + HCost;  // Total cost

    public Node(Vector3 position, NodeType nodeType)
    {
        Position = position;
        NodeType = nodeType;
        ResetNode();
    }

    /// <summary>
    /// Remove any pre existing pathfinding values within the node
    /// </summary>
    public void ResetNode()
    {
        GCost = int.MaxValue;
        Parent = null;
    }

    /// <summary>
    /// Set this node as the starting node in pathfinding
    /// </summary>
    /// <param name="dist">The distance of this node from the end node</param>
    public void SetAsStartNode(float dist)
    {
        GCost = 0;
        HCost = dist;
    }
}