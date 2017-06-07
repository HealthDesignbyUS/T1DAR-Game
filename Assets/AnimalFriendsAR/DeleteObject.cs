using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteObject : MonoBehaviour
{

    void OnTriggerEnter (Collider other)
    {
        Debug.Log ("triggered");
        switch (other.gameObject.tag) {
        case "Sugar":
        case "Insulin":
        case "Icecream":
        case "Milk":
        case "Cheerios":
        case "Cherries":
        case "Toast":
            spawnObject.S.numObjects--;
            Destroy (other.gameObject);
            break;
        }
    }
}
