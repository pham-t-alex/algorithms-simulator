using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisjointSetElement : MonoBehaviour
{
    private string elementName = "?";
    public string ElementName
    {
        get
        {
            return elementName;
        }
    }
    [SerializeField] private bool mouseTouching;
    public bool MouseTouching
    {
        get
        {
            return mouseTouching;
        }
    }

    private SetText textInput;
    private int rank;
    private int size;
    private DisjointSetElement rep;
    private DisjointSetArrow arrow;

    // Start is called before the first frame update
    void Start()
    {
        textInput = Instantiate(Resources.Load<GameObject>("Prefabs/SetText"), transform.position, Quaternion.identity, GameObject.Find("UIWorld").transform).GetComponent<SetText>();
        textInput.SetElement(this);
        rep = this;
        rank = 0;
        size = 1;
        arrow = Instantiate(Resources.Load<GameObject>("Prefabs/DisjointSetArrow"), GameObject.Find("Arrows").transform).GetComponent<DisjointSetArrow>();
        arrow.SetElements(this, this);
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

    public void UpdatePosition()
    {
        textInput.transform.position = transform.position;
        arrow.UpdatePosition();
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public void SetName(string name)
    {
        if (name == null || name == "")
        {
            elementName = "?";
        }
        else
        {
            elementName = name;
        }
    }

    public void CreateInfoText()
    {

    }

    public void DestroyInfoText()
    {

    }
}
