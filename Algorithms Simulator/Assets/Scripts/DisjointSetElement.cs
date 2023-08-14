using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
    public int Rank
    {
        get
        {
            return rank;
        }
    }
    private int size;
    public int Size
    {
        get
        {
            return size;
        }
    }
    private DisjointSetElement rep;
    public DisjointSetElement Rep
    {
        get
        {
            return rep;
        }
    }
    private DisjointSetArrow arrow;
    public DisjointSetArrow Arrow
    {
        get
        {
            return arrow;
        }
    }
    private List<DisjointSetArrow> incomingArrows = new List<DisjointSetArrow>();

    private GameObject infoText;
    private bool sizeRankUpdated;
    private bool repUpdated;

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
        sizeRankUpdated = false;
        repUpdated = false;
        incomingArrows.Add(arrow);
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
        foreach (DisjointSetArrow a in incomingArrows)
        {
            if (a.Source != this)
            {
                a.UpdatePosition();
            }
        }
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
        infoText = Instantiate(Resources.Load<GameObject>("Prefabs/InfoText"), transform.position, Quaternion.identity, GameObject.Find("Other").transform);
        string s = elementName + "\n";
        s += "Rep = " + rep.elementName;
        s += "\n";
        if (DisjointSetController.SetController.UsingRank)
        {
            s += "Rank = " + rank;
        }
        else
        {
            s += "Size = " + size;
        }
        infoText.GetComponent<TMP_Text>().text = s;
        textInput.gameObject.SetActive(false);
    }

    public void DestroyInfoText()
    {
        Destroy(infoText);
        infoText = null;
        textInput.gameObject.SetActive(true);
    }

    public void UpdateInfoText()
    {
        string s = elementName + "\n";
        if (repUpdated)
        {
            s += "Rep = <color=#6a00ffff>" + rep.elementName + "</color>";
        }
        else
        {
            s += "Rep = " + rep.elementName;
        }
        s += "\n";
        if (DisjointSetController.SetController.UsingRank)
        {
            if (sizeRankUpdated)
            {
                s += "Rank = <color=#6a00ffff>" + rank + "</color>";
            }
            else
            {
                s += "Rank = " + rank;
            }
        }
        else
        {
            if (sizeRankUpdated)
            {
                s += "Size = <color=#6a00ffff>" + size + "</color>";
            }
            else
            {
                s += "Size = " + size;
            }
        }
        sizeRankUpdated = false;
        repUpdated = false;
        infoText.GetComponent<TMP_Text>().text = s;
    }

    public void ChangeRep(DisjointSetElement e)
    {
        if (rep == e)
        {
            return;
        }
        rep.incomingArrows.Remove(arrow);
        rep = e;
        e.incomingArrows.Add(arrow);
        arrow.SetElements(this, e);
        arrow.UpdatePosition();
        repUpdated = true;
    }

    public void AddToSize(int addition)
    {
        size += addition;
        sizeRankUpdated = true;
    }

    public void IncrementRank()
    {
        rank++;
        sizeRankUpdated = true;
    }

    public void Reset()
    {
        size = 1;
        rank = 0;
        rep = this;
        arrow.SetElements(this, this);
        arrow.UpdatePosition();
        sizeRankUpdated = false;
        repUpdated = false;
    }

    public override string ToString()
    {
        return elementName;
    }
}
