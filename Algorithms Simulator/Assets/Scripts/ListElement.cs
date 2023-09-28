using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListElement : MonoBehaviour
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

    private Vector3 movement;

    private ListText textInput;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void InstantiateTextInput()
    {
        textInput = Instantiate(Resources.Load<GameObject>("Prefabs/ListText"), transform.position, Quaternion.identity, GameObject.Find("UIWorld").transform).GetComponent<ListText>();
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
    }

    public void SetValue(float f)
    {
        elementValue = f;
        if (textInput == null)
        {
            InstantiateTextInput();
        }
        textInput.GetComponent<TMPro.TMP_InputField>().text = elementValue.ToString();
        if (elementValue == (int) elementValue)
        {
            SortsController.ListSortController.RemoveNonIntegerElement(this);
        }
        else
        {
            SortsController.ListSortController.AddNonIntegerElement(this);
        }
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
        Destroy(textInput.gameObject);
        Destroy(gameObject);
    }

    public void SetMovement(Vector3 vector)
    {
        movement = vector;
    }

    public void Move()
    {
        transform.position += movement;
        UpdatePosition();
    }

    public override string ToString()
    {
        return elementValue.ToString();
    }
}
