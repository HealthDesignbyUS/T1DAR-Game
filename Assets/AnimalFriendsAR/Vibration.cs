using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour
{

    public static Vibration S;
    public AndroidJavaClass unityPlayer;
    public AndroidJavaObject currentActivity;
    public AndroidJavaObject vibrator;

    void Awake ()
    {
        S = this;
        if (isAndroid ()) {
            unityPlayer = new AndroidJavaClass ("com.unity3d.player.UnityPlayer");
            currentActivity = unityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
            vibrator = currentActivity.Call<AndroidJavaObject> ("getSystemService", "vibrator");
        }
    }

    // Use this for initialization
    void Start ()
    {
		
    }

    public void Vibrate (long milliseconds)
    {
        if (isAndroid ()) {
            vibrator.Call ("vibrate", milliseconds);
        } else {
            Handheld.Vibrate ();
        }
    }

    private bool isAndroid ()
    {
        #if UNITY_ANDROID && !UNITY_EDITOR
        return true;
        #else
        return false;
        #endif
    }
}
