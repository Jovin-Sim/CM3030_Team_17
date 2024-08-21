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
    public float GCost { get; set; }  // Cost from start to this node
    public float HCost { get; set; }  // Heuristic cost to end
    public float FCost => GCost + HCost;  // Total cost

    public Node(Vector3 position, NodeType nodeType)
    {
        Position = position;
        NodeType = nodeType;
        ResetNode();
    }

    public void ResetNode()
    {
        GCost = int.MaxValue;
        Parent = null;
    }

    public void SetAsStartNode(float dist)
    {
        GCost = 0;
        HCost = dist;
    }
}