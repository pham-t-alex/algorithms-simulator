using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Edge : MonoBehaviour
{
    private int weight;
    private bool mouseTouching;
    public bool MouseTouching
    {
        get
        {
            return mouseTouching;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseEnter()
    {
        mouseTouching = true;
    }

    private void OnMouseExit()
    {
        mouseTouching = false;
    }
}
