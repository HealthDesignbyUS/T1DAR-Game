using System.Linq;
using UnityEngine;

public class MoverTool {
    private const float GrabRadius = 0.025f;

    private readonly Rigidbody _rigidbody;
    private FixedJoint _fixedJoint;

    private readonly GameObject _visualizer;
    public MoverTool(Rigidbody rigidbody) {
        _rigidbody = rigidbody;
        _visualizer = CreateVisualizer(rigidbody.transform);
    }

    private static GameObject CreateVisualizer(Transform parent) {
        var visualizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Object.DestroyImmediate(visualizer.GetComponent<Collider>());
        visualizer.transform.parent = parent.transform;
        visualizer.transform.localPosition = Vector3.zero;
        visualizer.transform.localScale = Vector3.one * GrabRadius * 2;
        visualizer.gameObject.SetActive(false);
        return visualizer;
    }

    public void Grab() {
        _visualizer.SetActive(true);
        Collider first = Physics.OverlapSphere(_rigidbody.position, GrabRadius, 1 << 9).FirstOrDefault();
        if (!first) return;
        first.GetComponent<Rigidbody>().isKinematic = false;
        _fixedJoint = first.gameObject.AddComponent<FixedJoint>();
        _fixedJoint.connectedBody = _rigidbody;
    }

    public void Release(Vector3 velocity=default(Vector3), Vector3 angularVelocity=default(Vector3)) {
        _visualizer.SetActive(false);
        if (!_fixedJoint) return;
        Rigidbody otherRb = _fixedJoint.GetComponent<Rigidbody>();
        Object.Destroy(_fixedJoint);
        _fixedJoint = null;

        otherRb.isKinematic = velocity == default(Vector3) && angularVelocity == default(Vector3);
        if (!otherRb.isKinematic) {
            otherRb.velocity = velocity;
            otherRb.angularVelocity = angularVelocity;
            otherRb.maxAngularVelocity = otherRb.angularVelocity.magnitude;
        }
    }
}