using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cluster : MonoBehaviour
{
    public Vector3 Position { get; set; }
    public Dictionary<Vector3, Node> Nodes { get; private set; }
    public List<Node> Entrances { get; private set; }

    public Cluster(Vector3 position)
    {
        Position = position;
        Nodes = new Dictionary<Vector3, Node>();
        Entrances = new List<Node>();
    }

    public void AddNode(Node node)
    {
        Nodes[node.Position] = node;
    }

    public void AddEntrance(Node node)
    {
        if (!Entrances.Contains(node))
        {
            Entrances.Add(node);
        }
    }
}
