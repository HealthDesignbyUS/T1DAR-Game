using System.Collections;
//using UniRx;
using UnityEngine;
using Random = UnityEngine.Random;

public class DVDSpawner : MonoBehaviour {

    public float Scale = 1;
    public int Speed = 100;

    public Transform Prefab;

    void Start () {
        StartCoroutine(SpawnMethod());
    }

    public float Interval = 1f;
    public float IntervalAdjustRate = 0.95f;
    public float MinimumInterval = 0.2f;
    private IEnumerator SpawnMethod() {
        while (true) {
            yield return new WaitForSeconds(Interval);
            SpawnDVD();
            Interval *= IntervalAdjustRate;
            Interval = Mathf.Max(MinimumInterval, Interval);
        }
    }

    private void SpawnDVD() {
        var spawnPoint = transform.position + (Random.insideUnitSphere * Scale);
        Transform instantiate = Instantiate(Prefab, spawnPoint, Quaternion.identity);
        Rigidbody rb = instantiate.GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.AddForce(transform.forward * Speed);
        rb.AddTorque(Random.value * 20,Random.value * 20,Random.value * 20);
        Destroy(instantiate.gameObject, 30f);
    }

    void OnDrawGizmosSelected() {
        Gizmos.color = new Color(1, 1, 0, 0.75F);
        Gizmos.DrawSphere (transform.position, Scale);
    }
}
