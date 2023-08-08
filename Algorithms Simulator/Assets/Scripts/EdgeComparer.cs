using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeComparer : IComparer<Edge>
{
    public int Compare(Edge a, Edge b)
    {
        int comparison = a.Source.VertexName.CompareTo(b.Source.VertexName);
        if (comparison == 0)
        {
            return a.Destination.VertexName.CompareTo(b.Destination.VertexName);
        }
        else
        {
            return comparison;
        }
    }
}