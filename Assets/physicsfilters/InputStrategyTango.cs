using UnityEngine;

public class InputStrategy : MonoBehaviour {
    private MoverTool _tool;
    private Rigidbody _rigidbody;

    void Start() {
        Transform moverToolTransform = new GameObject("MoverTool").transform;
        moverToolTransform.parent = Camera.main.transform;

        _rigidbody = moverToolTransform.gameObject.AddComponent<Rigidbody>();
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;

        _tool = new MoverTool(_rigidbody);
    }

    void Update() {
        //        if (Input.touchCount == 1) {
        //Input.GetTouch(0)

        RaycastHit hitInfo;
        if (Input.GetMouseButtonDown(0)) {
            Debug.Log(Input.mousePosition);
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo)) {
                GameObject tapped = hitInfo.collider.gameObject;
                Debug.Log(tapped);

                _rigidbody.transform.position = hitInfo.point;
                _tool.Grab();
            }
        }

        if (Input.GetMouseButtonUp(0)) { _tool.Release(); }
    }
}