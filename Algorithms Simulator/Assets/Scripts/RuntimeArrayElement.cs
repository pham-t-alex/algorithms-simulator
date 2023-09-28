using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RuntimeArrayElement : MonoBehaviour
{
    private GameObject infoText;
    private Dictionary<string, object> variables = new Dictionary<string, object>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void CreateInfoText()
    {
        infoText = Instantiate(Resources.Load<GameObject>("Prefabs/InfoText"), transform.position, Quaternion.identity, GameObject.Find("Other").transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public void UpdateInfoText(string text, bool highlight)
    {
        string s = "";
        if (highlight)
        {
            s += "<color=#6a00ffff>";
        }
        s += text;
        if (highlight)
        {
            s += "</color>";
        }
        infoText.GetComponent<TMP_Text>().text = s;
    }

    public void AddVariable(string varName, object initialVal)
    {
        variables.Add(varName, initialVal);
    }

    public void SetVariable(string varName, object value)
    {
        variables[varName] = value;
    }

    public object GetVariable(string varName)
    {
        return variables[varName];
    }

    public void RemoveVariable(string varName)
    {
        variables.Remove(varName);
    }

    public void ClearVariables()
    {
        variables.Clear();
    }

    public void Destroy()
    {
        Destroy(infoText);
        Destroy(gameObject);
    }
}
