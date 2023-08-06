using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EdgeText : InputText
{
    private Edge edge;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetEdge(Edge e)
    {
        edge = e;
    }

    public void UpdateEdgeWeight(string s)
    {
        if (!edge.SetWeight(s))
        {
            GetComponent<TMPro.TMP_InputField>().text = "";
        }
    }

}
