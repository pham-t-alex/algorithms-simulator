using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public readonly struct Comparison
{
    public Comparison(float one, float two)
    {
        First = one;
        Second = two;
    }

    public float First { get; }
    public float Second { get; }

    public override string ToString() => $"{First} & {Second}";
}