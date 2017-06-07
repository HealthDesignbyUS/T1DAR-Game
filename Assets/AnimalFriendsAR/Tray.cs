using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tray : MonoBehaviour
{
    public static Tray S;

    private Vector3[] positions;
    private GameObject[] objOnTray;
    public float totalCarb;
    private int nextSlot;
    private float timer;
    public const int numObjOnTray = 12;
    public GameObject carbCountPanel;
    public GameObject clearButton;

    bool charSpawned;

    void Awake ()
    {
        S = this;
    }

    // Use this for initialization
    void Start ()
    {
        positions = new Vector3[numObjOnTray] {
            new Vector3 (-0.083f, 0.01f, 0.15f),
            new Vector3 (0.083f, 0.01f, 0.15f),
            new Vector3 (-0.25f, 0.01f, 0.15f),
            new Vector3 (0.25f, 0.01f, 0.15f),
            new Vector3 (-0.083f, 0.01f, 0f),
            new Vector3 (0.083f, 0.01f, 0f),
            new Vector3 (-0.25f, 0.01f, 0f),
            new Vector3 (0.25f, 0.01f, 0f),
            new Vector3 (-0.083f, 0.01f, -0.15f),
            new Vector3 (0.083f, 0.01f, -0.15f),
            new Vector3 (-0.25f, 0.01f, -0.15f),
            new Vector3 (0.25f, 0.01f, -0.15f)
        };
        objOnTray = new GameObject[numObjOnTray];
        totalCarb = 0;
        nextSlot = 0;
        timer = 0;
        carbCountPanel.SetActive (true);
    }
	
    // Update is called once per frame
    void Update ()
    {
        if (!charSpawned) {
            if (CharacterAttributes.S) {
                transform.position = CharacterAttributes.S.transform.position + CharacterAttributes.S.transform.forward * 0.7f;
                charSpawned = true;
            }
        }

        ++timer;
        if (timer == 50) {
            carbCountPanel.GetComponentInChildren<Text> ().text = "Total Carb: " + totalCarb.ToString ();
        } 

//        infoObj.transform.LookAt (Camera.main.transform);

        float thickness = 0.15f;
        Vector3 direction = gameObject.transform.position - Camera.main.transform.position;
        RaycastHit[] hits;
//        hits = Physics.RaycastAll (Camera.main.transform.position, direction, Mathf.Infinity);
        hits = Physics.SphereCastAll (Camera.main.transform.position, thickness, direction);

        for (int h = 0; h < hits.Length; h++) {
            GameObject obj = hits [h].collider.gameObject;
            switch (obj.tag) {
            case "Sugar":
            case "Icecream":
            case "Milk":
            case "Cheerios":
            case "Cherries":
            case "Toast":
            case "Juice":
                if (!obj.GetComponent<Food> ().onTray) {
                    if (nextSlot < numObjOnTray) {
                        Vibration.S.Vibrate (500);
                        objOnTray [nextSlot] = obj;
                        obj.transform.SetParent (transform);
                        obj.transform.localPosition = positions [nextSlot];
                        obj.GetComponent<Food> ().onTray = true;
                        totalCarb += obj.GetComponent<Food> ().amount;
                        nextSlot++;
                        carbCountPanel.GetComponentInChildren<Text> ().text = "Total Carb: " + totalCarb.ToString ();
                        clearButton.SetActive (true);
                    } else {
                        Vibration.S.Vibrate (500);
                        timer = 0;
                        carbCountPanel.GetComponentInChildren<Text> ().text = "Sorry, the tray is full.";
                    }
                } else {
                    for (int i = 0; i < nextSlot; i++) {
                        if (objOnTray [i] == obj) {
                            obj.transform.localPosition = positions [i];
                        }
                    }
                }
                break;
            default:
                break;
            }
        }

        for (int i = 0; i < nextSlot; i++) {
            if (objOnTray [i] != null) {
                objOnTray [i].transform.localPosition = positions [i];
            }
        }
    }

    public void clearTray ()
    {
        for (int i = 0; i < nextSlot; i++) {
            spawnObject.S.numObjects--;
            Destroy (objOnTray [i]);
            objOnTray [i] = null;
        }

        totalCarb = 0;
        nextSlot = 0;
        clearButton.SetActive (false);
        carbCountPanel.GetComponentInChildren<Text> ().text = "Total Carb: " + totalCarb.ToString ();
    }
}
