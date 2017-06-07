using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlucoseMeter : MonoBehaviour
{

    public float timer;
    public GameObject glucoseMeterText;
    public GameObject glucoseMeterPanel;
    public bool ready;
    private LayerMask playerLayer;

    void Start ()
    {
        timer = 0;
        playerLayer = LayerMask.GetMask ("Player");
    }

    // Update is called once per frame
    void Update ()
    {
        if (ready) {
            Vector3 direction = gameObject.transform.position - Camera.main.transform.position;
            RaycastHit hitInfo;
            if (Physics.Raycast (Camera.main.transform.position, direction, out hitInfo, Mathf.Infinity, playerLayer)) {
                CharacterAttributes.S.meterChecked++;
                timer = 0;
                glucoseMeterPanel.SetActive (true);
                glucoseMeterText.GetComponent<Text> ().text = "Blood Glucose Level: " + CharacterAttributes.S.bloodSugarLevel.ToString ("F0");
                /*
                CharacterAttributes.S.graphObj.GetComponent<graphDisp> ().clearGraph ();
                for (float i = 0; i < 256; i += 4.8f) { 
                    CharacterAttributes.S.graphObj.GetComponent<graphDisp> ().plotPoint (new Vector2 (i, CharacterAttributes.S.bloodSugarLevel));
                }
                */
                Vibration.S.Vibrate (500);
            }
        }

        ++timer;

        if (timer == 100f) {
            glucoseMeterPanel.SetActive (false);
        }
    }
}