using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterChooser : MonoBehaviour {
    void OnMouseDown() {
        PlayerPrefs.SetString("CharacterName", gameObject.name);
        SceneManager.LoadScene("AnimalFriendsAR");
    }
}