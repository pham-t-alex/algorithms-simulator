using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetText : InputText
{
    private DisjointSetElement element;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateElementName(string s)
    {
        element.SetName(s);
    }

    public void SetElement(DisjointSetElement e)
    {
        element = e;
    }
}
