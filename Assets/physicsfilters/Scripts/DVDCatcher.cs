using UnityEngine;

class DVDCatcher : MonoBehaviour {
    private int _layer = 8;


    void OnCollisionEnter(Collision c) {
        if (c.gameObject.layer == _layer) {
            Destroy(c.gameObject);
        }
    }
}