using UnityEngine;
using System.Linq;
using Tango;
using UnityEngine.UI;


public class CharacterPlacement : MonoBehaviour
{
    public GameObject[] Characters;
    public RectTransform TouchEffectPrefab;
    private TangoTouchProjector _tangoTouchProjector;

    public void Awake ()
    {
        var pointCloud = FindObjectOfType<TangoPointCloud> ();
        var tangoApplication = FindObjectOfType<TangoApplication> ();
        _tangoTouchProjector = new TangoTouchProjector (tangoApplication, pointCloud);
    }

    public void OnDestroy ()
    {
        _tangoTouchProjector.OnDestroy ();
    }

    public void Update ()
    {
        Vector3 touchPosition;
        if (!GetTouchPosition (out touchPosition))
            return;

        PlayTouchEffect (touchPosition);
        if (!TryTouchExistingCharacter (touchPosition)) {
            StartCoroutine (_tangoTouchProjector.Touch (touchPosition, ArTouchAction));
        }
    }

    bool GetTouchPosition (out Vector3 touchPosition)
    {
//        if (Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began) {
//            touchPosition = Input.GetTouch(0).position;
//            return true;
//        }
        if (Input.GetMouseButtonDown (0)) {
            touchPosition = Input.mousePosition;
            return true;
        }
        touchPosition = default(Vector3);
        return false;
    }

    private GameObject TryTouchExistingCharacter (Vector3 touchPosition)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast (Camera.main.ScreenPointToRay (touchPosition), out hitInfo)) {
            return hitInfo.collider.gameObject;
        }
        return null;
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

    private void ArTouchAction (Vector3 planeCenter, Quaternion lookToCamera, int status)
    {
        var prefab = Characters.FirstOrDefault (c => c.name == PlayerPrefs.GetString ("CharacterName"));
        if (!prefab) {
            return;
        }
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
        Instantiate (prefab, planeCenter, lookToCamera);
        CharacterAttributes.S.destPos = planeCenter;
        CharacterAttributes.S.destRot = lookToCamera;
        Destroy (this.gameObject);
    }

}