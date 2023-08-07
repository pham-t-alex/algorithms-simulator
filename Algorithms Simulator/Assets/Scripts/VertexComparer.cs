using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexComparer : IComparer<GraphVertex>
{
    public int Compare(GraphVertex a, GraphVertex b)
    {
        return a.VertexName.CompareTo(b.VertexName);
    }
}
