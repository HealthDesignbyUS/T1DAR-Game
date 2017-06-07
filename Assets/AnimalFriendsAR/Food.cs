using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : MonoBehaviour
{
    public float amount;
    public float interval;
    public bool ready;
    public bool onTray;
    public GameObject carbObj;
    private LayerMask playerLayer;

    // Use this for initialization
    void Start ()
    {
        playerLayer = LayerMask.GetMask ("Player");
        carbObj.GetComponentInChildren<TextMesh> ().text = amount.ToString ();
    }

    // Update is called once per frame
    void Update ()
    {
        carbObj.transform.LookAt (Camera.main.transform);
        if (LevelTemplate.S.curLevel != 2) {
            if (ready) {
                Vector3 direction = gameObject.transform.position - Camera.main.transform.position;
                RaycastHit hitInfo;
                if (Physics.Raycast (Camera.main.transform.position, direction, out hitInfo, Mathf.Infinity, playerLayer)) {
                    //CharacterAttributes.S.changeBS (amount, interval);
                    CharacterAttributes.S.glucodyn.AddCarbs (interval, amount);
                    CharacterAttributes.S.updateLineGraph ();
                    CharacterAttributes.S.foodEaten++;
                    if (this.gameObject.tag == "Sugar") {
                        CharacterAttributes.S.sugarTaken++;
                    }
                    Vibration.S.Vibrate (500);
                    spawnObject.S.numObjects--;
                    Destroy (this.gameObject);
                }
            }
        }	
    }
}
