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
            GraphController.GraphControl.Freeze();
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "DisjointSet")
        {
            DisjointSetController.SetController.Freeze();
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Heaps")
        {
            HeapController.HeapMainController.Freeze();
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Sorts")
        {
            SortsController.ListSortController.Freeze();
        }
    }

    public void Unfreeze()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "GraphAlgs")
        {
            GraphController.GraphControl.Unfreeze();
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "DisjointSet")
        {
            DisjointSetController.SetController.Unfreeze();
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Heaps")
        {
            HeapController.HeapMainController.Unfreeze();
        }
        else if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().name == "Sorts")
        {
            SortsController.ListSortController.Unfreeze();
        }
    }
}
