using UnityEngine;

public class ShapeTool : MonoBehaviour {

    public Mesh mesh;
    public Vector3 scale = new Vector3(0.1f, 0.1f, 0.1f);

    void OnTriggerEnter(Collider c) {
        Rigidbody rb = c.GetComponent<Rigidbody>();
        Physics.IgnoreCollision(c, GetComponent<Collider>());

        if (rb && rb.gameObject.layer == 8) {
            c.GetComponent<MeshFilter>().mesh = Instantiate(mesh);
            rb.transform.localScale = scale;
        }
    }
}