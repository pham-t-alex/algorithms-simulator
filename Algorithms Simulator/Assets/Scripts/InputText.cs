using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputText : MonoBehaviour
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
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GraphAlgs")
        {
            DirectedGraphController.DirGraphController.Freeze();
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "DisjointSet")
        {
            DisjointSetController.SetController.Freeze();
        }
    }

    public void Unfreeze()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GraphAlgs")
        {
            DirectedGraphController.DirGraphController.Unfreeze();
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "DisjointSet")
        {
            DisjointSetController.SetController.Unfreeze();
        }
    }
}
