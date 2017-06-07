using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitButton : MonoBehaviour {
    private const float UiButtonSizeX = 250.0f;
    private const float UiButtonSizeY = 130.0f;
    private const float UiButtonGapX = 5.0f;
    private const int UiFontSize = 25;


    public void OnGUI() {
        Rect distortionButtonRec = new Rect(UiButtonGapX,
            Screen.height - UiButtonSizeY - UiButtonGapX,
            UiButtonSizeX,
            UiButtonSizeY);

        if (GUI.Button(distortionButtonRec, "<size=" + UiFontSize + ">Exit</size>")) {
            SceneManager.LoadScene("DetectTangoCore");
        }    
    }
}