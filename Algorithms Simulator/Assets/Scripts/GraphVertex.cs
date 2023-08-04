using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphVertex : MonoBehaviour
{
    private string vertexName;
    private Dictionary<string, object> variables;
    private VertexText nameInput;
    [SerializeField] private bool mouseTouching;
    public bool MouseTouching
    {
        get
        {
            return mouseTouching;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        nameInput = Instantiate(Resources.Load<GameObject>("Prefabs/VertexText"), transform.position, Quaternion.identity, GameObject.Find("UIWorld").transform).GetComponent<VertexText>();
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
        nameInput.transform.position = transform.position;
    }
}
