using UnityEngine;

public class KillBoundaries : MonoBehaviour {

    void OnCollisionEnter(Collision c) {
        if (c.gameObject.layer == 8) {
            Destroy(c.gameObject);
        }
    }
}
