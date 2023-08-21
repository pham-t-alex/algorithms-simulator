using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapElement : MonoBehaviour
{
    private float elementValue = 0;
    public float ElementValue
    {
        get
        {
            return elementValue;
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

    private HeapConnector heapConnector;
    public HeapConnector HeapElementConnector
    {
        get
        {
            return heapConnector;
        }
    }

    private List<HeapConnector> incomingConnectors = new List<HeapConnector>();

    private HeapText textInput;
    // Start is called before the first frame update
    void Start()
    {

    }

    public void InstantiateTextInput()
    {
        textInput = Instantiate(Resources.Load<GameObject>("Prefabs/HeapText"), transform.position, Quaternion.identity, GameObject.Find("UIWorld").transform).GetComponent<HeapText>();
        textInput.SetElement(this);
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
        if (heapConnector != null)
        {
            heapConnector.UpdatePosition();
        }
        foreach (HeapConnector h in incomingConnectors)
        {
            if (h.Source != this)
            {
                h.UpdatePosition();
            }
        }
    }

    public void CreateConnector(HeapElement parent)
    {
        heapConnector = Instantiate(Resources.Load<GameObject>("Prefabs/HeapConnector"), GameObject.Find("Connectors").transform).GetComponent<HeapConnector>();
        heapConnector.SetElements(this, parent);
        parent.incomingConnectors.Add(heapConnector);
        heapConnector.UpdatePosition();
    }

    public void RemoveIncomingConnector(HeapConnector connector)
    {
        incomingConnectors.Remove(connector);
    }

    public void SetValue(float f)
    {
        elementValue = f;
        if (textInput == null)
        {
            InstantiateTextInput();
        }
        textInput.GetComponent<TMPro.TMP_InputField>().text = elementValue.ToString();

        HeapController.HeapMainController.UpdateHeapAsArray(null);
    }

    public void ChangeValue(float f)
    {
        elementValue = f;
        textInput.GetComponent<TMPro.TMP_InputField>().text = elementValue.ToString();
    }

    public void SetColor(Color color)
    {
        GetComponent<SpriteRenderer>().color = color;
    }

    public void CreateInfoText()
    {
        textInput.GetComponent<TMPro.TMP_InputField>().interactable = false;
    }

    public void DestroyInfoText()
    {
        textInput.GetComponent<TMPro.TMP_InputField>().interactable = true;
    }

    public void Destroy()
    {
        if (heapConnector != null)
        {
            Destroy(heapConnector);
        }
        Destroy(textInput.gameObject);
        Destroy(gameObject);
    }
}
