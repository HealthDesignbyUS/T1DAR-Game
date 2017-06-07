using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsTool : MonoBehaviour {
    public bool TurnOnGravity = false;
    public bool TurnOffGravity = false;

    public float scale = 1;
    public Vector3 impulse = Vector3.zero;
    private Material _material;

    // Use this for initialization
	void Start () {
	    _material = GetComponent<Renderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void OnTriggerEnter(Collider c) {
        Rigidbody rb = c.GetComponent<Rigidbody>();
        Physics.IgnoreCollision(c, GetComponent<Collider>());

        if (rb && rb.gameObject.layer == 8) {
            rb.useGravity = TurnOnGravity ? true : rb.useGravity;
            rb.useGravity = TurnOffGravity ? false : rb.useGravity;

            rb.transform.localScale *= scale;
            rb.AddForce(transform.TransformVector(impulse));
            rb.GetComponent<Renderer>().material.color = _material.color;
        }
    }
}