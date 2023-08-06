using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexText : InputText
{
    private GraphVertex vertex;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateVertexName(string s)
    {
        vertex.SetName(s);
    }

    public void SetVertex(GraphVertex v)
    {
        vertex = v;
    }
}
