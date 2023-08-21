using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow<T> : Connector<T> where T : MonoBehaviour
{
    public override void UpdatePosition()
    {
        if (source != destination)
        {
            transform.position = (source.transform.position + destination.transform.position) / 2f;
            Vector3 direction = (destination.transform.position - source.transform.position).normalized;
            float angle = Mathf.Asin(direction.y);
            if (direction.x < 0)
            {
                angle = Mathf.PI - angle;
            }
            transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
            float scale = (Vector3.Distance(source.transform.position, destination.transform.position) - 2) / 5f;
            transform.localScale = new Vector3(scale, 1, 1);
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/directededge");
        }
        else
        {
            transform.position = source.transform.position + new Vector3(1, 1, 0);
            transform.rotation = Quaternion.identity;
            transform.localScale = Vector3.one;
            GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Sprites/directedselfedge");
        }
    }
}
