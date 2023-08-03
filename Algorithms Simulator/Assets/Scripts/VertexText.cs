using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VertexText : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Freeze()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "DirectedGraphAlgs")
        {
            DirectedGraphController.DirGraphController.Freeze();
        }
    }

    public void Unfreeze()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "DirectedGraphAlgs")
        {
            DirectedGraphController.DirGraphController.Unfreeze();
        }
    }
}
