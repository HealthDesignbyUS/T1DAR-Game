using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class transitionHandler : MonoBehaviour {

    string nextScene;
    int levelPoints;
    int curLevel;

    public GameObject displayText;

	// Use this for initialization
	void Start () {
        curLevel = PlayerPrefs.GetInt("Current Level", 2);
        levelPoints = PlayerPrefs.GetInt("Net Score", 0);
        displayText.GetComponent<Text>().text = "Congratulations, you earned " + levelPoints + " points.";

        Invoke("jumpToScene", 2f);
    }
	
	// Update is called once per frame
	void jumpToScene () {
        SceneManager.LoadScene("AnimalFriendsAR_lvl" + curLevel);
    }
}
