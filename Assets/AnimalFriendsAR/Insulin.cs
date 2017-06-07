using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Insulin : MonoBehaviour
{
    public bool ready;
    public GameObject insulinObj;
    private LayerMask playerLayer;

    // Use this for initialization
    void Start ()
    {
        playerLayer = LayerMask.GetMask ("Player");
        insulinObj.GetComponentInChildren<TextMesh> ().text = "1";
    }

    // Update is called once per frame
    void Update ()
    {
        insulinObj.transform.LookAt (Camera.main.transform);
        if (ready) {
            Vector3 direction = gameObject.transform.position - Camera.main.transform.position;
            RaycastHit hitInfo;
            if (Physics.Raycast (Camera.main.transform.position, direction, out hitInfo, Mathf.Infinity, playerLayer)) {
                //CharacterAttributes.S.changeBS (-50, 15f);
                CharacterAttributes.S.glucodyn.AddInsulin (CharacterAttributes.S.curTimeTick, CharacterAttributes.S.curTimeTick + 30f);
                CharacterAttributes.S.updateLineGraph ();
                CharacterAttributes.S.insulinTaken++;
                Vibration.S.Vibrate (500);
                spawnObject.S.numObjects--;
                Destroy (this.gameObject);
            }
        }
    }
}
