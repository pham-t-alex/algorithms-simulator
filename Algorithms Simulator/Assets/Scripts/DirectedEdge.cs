using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DirectedEdge : Edge
{
    private GraphVertex source;
    private GraphVertex destination;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/EdgeText"), transform.position, Quaternion.identity, GameObject.Find("UIWorld").transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}