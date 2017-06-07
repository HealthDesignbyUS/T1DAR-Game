using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tango;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    public const int maxDraggableItems = 3;
    public RectTransform TouchEffectPrefab;

    private int itemsDragging = 0;
    private Vector3[] screenPoint = new Vector3[maxDraggableItems];
    private Vector3[] offset = new Vector3[maxDraggableItems];
    private GameObject[] selectedObj = new GameObject[maxDraggableItems];
    private TangoTouchProjector _tangoTouchProjector;
    private LayerMask layer;

    void Awake ()
    {
        var pointCloud = FindObjectOfType<TangoPointCloud> ();
        var tangoApplication = FindObjectOfType<TangoApplication> ();
        _tangoTouchProjector = new TangoTouchProjector (tangoApplication, pointCloud);
        layer = ~LayerMask.GetMask ("Player");
    }

    void OnDestroy ()
    {
        _tangoTouchProjector.OnDestroy ();
    }

    // Update is called once per frame
    void Update ()
    {
        foreach (Touch touch in Input.touches) {
            int i = touch.fingerId;
            Ray ray = Camera.main.ScreenPointToRay (touch.position);
            RaycastHit hit;
            switch (touch.phase) {
            case TouchPhase.Began:
                if (Physics.Raycast (ray, out hit, Mathf.Infinity, layer)) {
//                if (Physics.Raycast (ray, out hit, 100, layer)) {
                    if (hit.collider != null) {
                        screenPoint [i] = Camera.main.WorldToScreenPoint (hit.transform.gameObject.transform.position);
                        offset [i] = hit.transform.gameObject.transform.position - Camera.main.ScreenToWorldPoint (new Vector3 (touch.position.x, touch.position.y, screenPoint [i].z));
                        selectedObj [i] = hit.transform.gameObject;
                        if (selectedObj [i].tag == "Icecream" || selectedObj [i].tag == "Milk" || selectedObj [i].tag == "Sugar") {
                            selectedObj [i].GetComponent<Food> ().ready = true;
                        } else if (selectedObj [i].tag == "Insulin") {
                            selectedObj [i].GetComponent<Insulin> ().ready = true;
                        } else if (selectedObj [i].tag == "Meter") {
                            selectedObj [i].GetComponent<GlucoseMeter> ().ready = true;
                        }
                    }
                } else {
                    if (LevelTemplate.S.curLevel != 2) {
                        PlayTouchEffect (touch.position);
                        StartCoroutine (_tangoTouchProjector.Touch (touch.position, SetCharPos));
                    }
                }
                break;
            case TouchPhase.Moved:
                if (selectedObj [i] != null) {
                    Vector3 curScreenPoint = new Vector3 (touch.position.x, touch.position.y, screenPoint [i].z);
                    Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset [i];
                    selectedObj [i].transform.position = curPosition;
                    print ("Obj " + i + " dragged");
                }
                break;
            case TouchPhase.Ended:
                if (selectedObj [i].tag == "Icecream" || selectedObj [i].tag == "Milk" || selectedObj [i].tag == "Sugar") {
                    selectedObj [i].GetComponent<Food> ().ready = false;
                } else if (selectedObj [i].tag == "Insulin") {
                    selectedObj [i].GetComponent<Insulin> ().ready = false;
                } else if (selectedObj [i].tag == "Meter") {
                    selectedObj [i].GetComponent<GlucoseMeter> ().ready = false;
                }
                selectedObj [i] = null;
                break;
            }
        }
    }

    private void SetCharPos (Vector3 planeCenter, Quaternion lookToCamera, int status)
    {
        if (status == 0) {
            Hud.S.DisplayOrientationError ();
            return;
        }

        if (status == 2) {
            Hud.S.DisplayCharTooFar ();
            return;
        }
             
        if (Vector3.Distance (Camera.main.transform.position, planeCenter) <= 1f) {
            Hud.S.DisplayCharTooClose ();
            return;
        }
        CharacterAttributes.S.destPos = planeCenter;
        CharacterAttributes.S.destRot = lookToCamera;
    }

    private void PlayTouchEffect (Vector3 touchPosition)
    {
        var canvas = FindObjectOfType<Canvas> ();
        RectTransform touchEffectRectTransform = (RectTransform)Instantiate (TouchEffectPrefab);
        touchEffectRectTransform.transform.SetParent (canvas.transform, false);
        Vector2 normalizedPosition = touchPosition;
        normalizedPosition.x /= Screen.width;
        normalizedPosition.y /= Screen.height;
        touchEffectRectTransform.anchorMin = touchEffectRectTransform.anchorMax = normalizedPosition;
    }
}
