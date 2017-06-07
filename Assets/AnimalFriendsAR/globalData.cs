using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class globalData : MonoBehaviour
{

    static public globalData S;
    public int score;

    // Use this for initialization
    void Awake ()
    {
        S = this;
        score = PlayerPrefs.GetInt ("Player Score", 0);
    }

    private void OnDestroy ()
    {
        PlayerPrefs.SetInt ("Player Score", score);
    }

    private void OnApplicationQuit ()
    {
        PlayerPrefs.SetInt ("Player Score", score);
    }
}
