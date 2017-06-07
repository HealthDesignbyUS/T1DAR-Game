using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hud : MonoBehaviour
{
    public static Hud S;
    public GameObject bsText;
    public GameObject statusText;
    public GameObject posText;
    public GameObject posPanel;
    public GameObject scoreText;
    private float posTimer = 0f;

    void Awake ()
    {
        S = this;
    }
	
    // Update is called once per frame
    void Update ()
    {
        int globalScore = globalData.S.score;
        int localScore = 0;
        if (CharacterAttributes.S) {
            localScore = CharacterAttributes.S.currentScore;
        }

        scoreText.GetComponent<Text> ().text = "Score: " + globalScore + localScore;


        if (posTimer < 100f) {
            posTimer++;
            if (posTimer == 100f) {
                posPanel.SetActive (false);
            }
        }
        if (!CharacterAttributes.S) {
            return;
        }
        if (CharacterAttributes.S.bloodSugarLevel > 300) {
            statusText.GetComponent<Text> ().text = "VERY HIGH";
        } else if (CharacterAttributes.S.bloodSugarLevel > 150) { 
            statusText.GetComponent<Text> ().text = "HIGH";
        } else if (CharacterAttributes.S.bloodSugarLevel < 30) {
            statusText.GetComponent<Text> ().text = "VERY LOW";
        } else if (CharacterAttributes.S.bloodSugarLevel < 70) {
            statusText.GetComponent<Text> ().text = "LOW";
        } else {
            statusText.GetComponent<Text> ().text = "NORMAL";
        }
    }

    public void DisplayCharTooClose ()
    {
        posText.GetComponent<Text> ().text = "You're a little too close.";
        posPanel.SetActive (true);
        Vibration.S.Vibrate (500);
        posTimer = 0;
    }

    public void DisplayCharTooFar ()
    {
        posText.GetComponent<Text> ().text = "Sorry, it's too far for your pet to move there";
        posPanel.SetActive (true);
        Vibration.S.Vibrate (500);
        posTimer = 0;
    }

    public void DisplayOrientationError ()
    {
        posText.GetComponent<Text> ().text = "Sorry, your pet can't stand on the wall or ceiling.";
        posPanel.SetActive (true);
        Vibration.S.Vibrate (500);
        posTimer = 0;
    }
}
