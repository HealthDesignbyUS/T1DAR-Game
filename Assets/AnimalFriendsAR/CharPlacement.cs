using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharPlacement : MonoBehaviour {

	public GameObject character;
	private TangoPointCloud m_pointCloud;

	// Use this for initialization
	void Start () {
		m_pointCloud = FindObjectOfType<TangoPointCloud> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.touchCount == 1) {
			// Trigger place char function when single touch ended.
			Touch t = Input.GetTouch (0);
			if (t.phase == TouchPhase.Ended) {
				PlaceChar (t.position);
			}
		}
	}

	void PlaceChar(Vector2 touchPos) {
		// Find the plane
		Camera cam = Camera.main;
		Vector3 planeCenter;
		Plane plane;
		if (!m_pointCloud.FindPlane (cam, touchPos, out planeCenter, out plane)) {
			Debug.Log ("cannot find plane.");
			return;
		}

		// Place char on the surface, and make it always face the camera.
		if (Vector3.Angle (plane.normal, Vector3.up) < 30.0f) {
			Vector3 up = plane.normal;
			Vector3 right = Vector3.Cross (plane.normal, cam.transform.forward).normalized;
			Vector3 forward = Vector3.Cross (right, plane.normal).normalized;
			Instantiate (character, planeCenter, Quaternion.LookRotation (forward, up));
		} else {
			Debug.Log ("surface is too steep for character to stand on.");
		}
	}
}
