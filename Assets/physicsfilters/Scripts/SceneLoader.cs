using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour {

	void Awake () {
        SceneManager.LoadScene(1, LoadSceneMode.Additive);
	    gameObject.AddComponent(typeof(InputStrategy));
	}
}
