using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelTemplate : MonoBehaviour
{

    static public LevelTemplate S;

    public int curLevel;

    public int curPhase = 0;
    public int foodNeeded = 5;
    public int insulinNeeded = 5;
    public int meterCheckNeeded = 3;
    public bool levelComplete;

    public GameObject tutText;
    public GameObject darkPanel;
    public GameObject timeText;
    public GameObject carbCountPanel;

    public bool isRunning;

    private int prevLevel = 1;

    // Use this for initialization
    void Awake ()
    {
        S = this;
        updateTutorialObj ();
    }
	
    // Update is called once per frame
    void Update ()
    {
        if (curLevel == 1) {
            level1 ();
        } else if (curLevel == 2) {
            if (prevLevel != curLevel) {
                updateTutorialObj ();
                prevLevel = curLevel;
            }
            level2 ();
        }
    }

    void updateTutorialObj ()
    {
        print ("UPDATING");
        tutText = GameObject.Find ("TutorialText");
    }

    void level1 ()
    {
        if (curPhase == 0) {
            if (CharacterAttributes.S) {
                tutText.GetComponent<Text> ().text = "Great! Now you can move your pet by tapping on the floor or any horizontal planes.";
                CharacterAttributes.S.moved = false;
                curPhase++;
                CharacterAttributes.S.addPoints (10);
            }
        } else if (curPhase == 1) {
            if (CharacterAttributes.S.moved) {
                tutText.GetComponent<Text> ().text = "Your pet seems to be hungry... Try feeding them some food items.";
                //CharacterAttributes.S.foodEaten = 0;
                curPhase++;
                CharacterAttributes.S.addPoints (10);
            }
        } else if (curPhase == 2) { //eat 5 food

            if (CharacterAttributes.S.foodEaten >= 1 && CharacterAttributes.S.foodEaten < foodNeeded) {
                tutText.GetComponent<Text> ().text = "Good work! Your pet still seems to be hungry however, so feed them " + (foodNeeded - CharacterAttributes.S.foodEaten) + " more food items.";
            } else if (CharacterAttributes.S.foodEaten >= foodNeeded) {
                CharacterAttributes.S.startTime = -20f;
                CharacterAttributes.S.updateBS ();
                tutText.GetComponent<Text> ().text = "Oh dear, it seems that your pet seems to be High from eating all those carbohydrates! Check their blood sugar level with the Meter!";
                CharacterAttributes.S.meterChecked = 0;
                curPhase++;
                CharacterAttributes.S.addPoints (50);
                CharacterAttributes.S.insulinTaken = 0;
                //CharacterAttributes.S.bloodSugarLevel = Mathf.Max (200, CharacterAttributes.S.bloodSugarLevel);
            }
        } else if (curPhase == 3) { // Check blood sugar level with meter
            if (CharacterAttributes.S.meterChecked >= 1) {
                tutText.GetComponent<Text> ().text = "Lower your pet's blood sugar level by taking an insulin shot.";
                //CharacterAttributes.S.insulinTaken = 0;
                curPhase++;
                CharacterAttributes.S.addPoints (20);
            }
        } else if (curPhase == 4) { //take 5 insulins
            if (CharacterAttributes.S.insulinTaken >= 1 && CharacterAttributes.S.insulinTaken < insulinNeeded) {
                tutText.GetComponent<Text> ().text = "Good work! Your pet still needs to take " + (insulinNeeded - CharacterAttributes.S.insulinTaken) + " more insulin shots.";
            } else if (CharacterAttributes.S.insulinTaken >= insulinNeeded) {
                tutText.GetComponent<Text> ().text = "Oh dear, it seems that your pet seems to be Low from taking too much insulin! Check their blood sugar level with the Meter!";
                CharacterAttributes.S.startTime = -200f;
                CharacterAttributes.S.updateBS ();
                CharacterAttributes.S.meterChecked = 0;
                curPhase++;
                CharacterAttributes.S.addPoints (50);
                CharacterAttributes.S.sugarTaken = 0;
            }
        } else if (curPhase == 5) { // Check blood sugar level with meter
            if (CharacterAttributes.S.meterChecked >= 1) {
                tutText.GetComponent<Text> ().text = "Feed your pet some sugar to bring their blood sugar back up to a normal level.";
                //CharacterAttributes.S.sugarTaken = 0;
                curPhase++;
                CharacterAttributes.S.addPoints (30);
            }
        } else if (curPhase == 6) { //take sugar

            if (CharacterAttributes.S.sugarTaken >= 1 && CharacterAttributes.S.bloodSugarLevel >= 70) {
                tutText.GetComponent<Text> ().text = "Now check your pet's blood sugar with the meter one last time to make sure things are normal.";
                CharacterAttributes.S.startTime = -300f;
                CharacterAttributes.S.updateBS ();
                CharacterAttributes.S.meterChecked = 0;
                curPhase++;
                CharacterAttributes.S.addPoints (20);
            }
        } else if (curPhase == 7) { // Check blood sugar level with meter
            if (CharacterAttributes.S.meterChecked >= 1) {
                curPhase++;
                CharacterAttributes.S.addPoints (20);
            }
        } else if (curPhase == 8) { // level complete
            tutText.GetComponent<Text> ().text = "Level Complete!";
            if (!levelComplete) {
                CharacterAttributes.S.addPoints (100);
                globalData.S.score += CharacterAttributes.S.currentScore;
                PlayerPrefs.SetInt ("Net Score", CharacterAttributes.S.currentScore);
                CharacterAttributes.S.currentScore = 0;
                PlayerPrefs.SetInt ("Current Level", curLevel + 1);
                StartCoroutine ("LevelTransition");
                levelComplete = true;
            }
        }
    }

    void level2 ()
    {
        if (curPhase == 0) {
            tutText.GetComponent<Text> ().text = "Touch the floor where you want your pet to arrive.";
            if (CharacterAttributes.S) {
                curPhase++;
                CharacterAttributes.S.addPoints (10);
            }
        } else if (curPhase == 1) {
            tutText.GetComponent<Text> ().text = "Did you notice that eating carbohydrate makes your blood sugar go up? Different foods have different types of carb ratios. For example, 1 slice of bread has 15 gm of carb; look around to see the carb amounts for different types of foods.";
            if (!isRunning) {
                StartCoroutine ("incrementPhaseDelay", 3f);
            }
        } else if (curPhase == 2) {
            tutText.GetComponent<Text> ().text = "Now let's have some breakfast! Make a meal by putting 100 carbs of food on the tray.";
            //once 100 carbs then move on
            if (Tray.S.totalCarb == 100) {
                curPhase++;
                carbCountPanel.SetActive (false);
                CharacterAttributes.S.insulinTaken = 0;
                CharacterAttributes.S.addPoints (50);
            }
        } else if (curPhase == 3) {
            tutText.GetComponent<Text> ().text = "Good work! However, before your pet eats you should give them 7 shots of insulin to counteract the 100 carbs they are about to eat.";
            if (CharacterAttributes.S.insulinTaken == 7) {
                curPhase++;
                CharacterAttributes.S.glucodyn.AddCarbs (10f, 105f);
                CharacterAttributes.S.updateLineGraph ();
                CharacterAttributes.S.addPoints (50);
                Tray.S.clearTray ();
            }
        } else if (curPhase == 4) {
            tutText.GetComponent<Text> ().text = "Your pet seems to be satisfied. Let's check back in a few hours for lunch.";
            darkPanel.SetActive (true);
            if (!isRunning) {
                StartCoroutine ("incrementPhaseDelay", 2f);
                CharacterAttributes.S.meterChecked = 0;
            }
        } else if (curPhase == 5) {
            darkPanel.SetActive (false);
            timeText.GetComponent<Text> ().text = "12:00 pm";
            CharacterAttributes.S.startTime -= 14400f;
            CharacterAttributes.S.updateBS ();
            tutText.GetComponent<Text> ().text = "Now check your pet's blood sugar with the meter one last time to make sure things are normal.";
            if (CharacterAttributes.S.meterChecked > 0) {
                curPhase++;
                CharacterAttributes.S.addPoints (20);
            }
        } else if (curPhase == 6) {
            tutText.GetComponent<Text> ().text = "Level Complete!";
            StartCoroutine ("ResetGame");
        }
    }

    IEnumerator ResetGame ()
    {
        yield return new WaitForSeconds (2);
        SceneManager.LoadScene ("AnimalFriendsChooser");
        yield break;
    }

    IEnumerator NextLevel ()
    {
        yield return new WaitForSeconds (2);
        SceneManager.LoadScene ("AnimalFriendsAR_lvl" + (curLevel + 1));
        yield break;
    }

    IEnumerator LevelTransition ()
    {
        yield return new WaitForSeconds (2);
        SceneManager.LoadScene ("SceneTransition");
        yield break;
    }

    IEnumerator incrementPhaseDelay (float time)
    {
        isRunning = true;
        yield return new WaitForSeconds (time);
        curPhase++;
        print ("INCREMENTED CURPHASE");
        isRunning = false;
        yield break;
    }
}
