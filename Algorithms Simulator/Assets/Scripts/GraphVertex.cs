using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraphVertex : MonoBehaviour
{
    private string vertexName;
    private Dictionary<string, object> variables;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(Resources.Load<GameObject>("Prefabs/VertexText"), transform.position, Quaternion.identity, GameObject.Find("UIWorld").transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
