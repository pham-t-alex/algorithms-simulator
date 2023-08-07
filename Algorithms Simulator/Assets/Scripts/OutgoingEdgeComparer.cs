using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutgoingEdgeComparer : IComparer<Edge>
{
    public int Compare(Edge a, Edge b)
    {
        return a.Destination.VertexName.CompareTo(b.Destination.VertexName);
    }
}