using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeWeightComparer : IComparer<Edge>
{
    public int Compare(Edge a, Edge b)
    {
        return a.Weight.CompareTo(b.Weight);
    }
}