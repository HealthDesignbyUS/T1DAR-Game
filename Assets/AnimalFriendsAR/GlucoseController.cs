using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlucoseController : MonoBehaviour
{

    public Image glucoseBar;
    //	public float glucoseAmt = 50;
    //	float time = 0;


    // Use this for initialization
    void Start ()
    {
        if (CharacterAttributes.S) {
            glucoseBar.fillAmount = CharacterAttributes.S.bloodSugarLevel / 240;
        }
    }
	
    // Update is called once per frame
    void Update ()
    {
//		time += 1;
//		if (time % 100 == 0) {
//			CharacterAttributes.S.bloodSugarLevel -= 1.0f;
//		}
        if (CharacterAttributes.S) {
            glucoseBar.fillAmount = CharacterAttributes.S.bloodSugarLevel / 240;
        }
    }
}
