using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListText : InputText
{
    private ListElement element;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateElementValue(string s)
    {
        float newWeight;

        bool success = float.TryParse(s, out newWeight);
        if (success)
        {
            element.SetValue(newWeight);
        }
        else
        {
            GetComponent<TMPro.TMP_InputField>().text = element.ElementValue.ToString();
        }
    }

    public void SetElement(ListElement e)
    {
        element = e;
    }
}
