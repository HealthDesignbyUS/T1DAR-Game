using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class graphDisp : MonoBehaviour {

    Material mat;

	// Use this for initialization
	void Start () {
        Texture2D newTex = new Texture2D(256, 256);
        GetComponent<RawImage>().texture = newTex;
    }

    public void clearGraph()
    {
        Texture2D newTex = new Texture2D(256, 256);
        GetComponent<RawImage>().texture = newTex;
    }

    public void plotGraph(Vector2[] points)
    {

        Texture2D newTex = new Texture2D(256, 256);
        GetComponent<RawImage>().texture = newTex;

        for (int i = 0; i < points.Length && i < 256; i++)
        {
            float xRaw = points[i].x;
            float yRaw = points[i].y;

            int xVal = (int)Mathf.Min(Mathf.Max(xRaw / 4.8f , 1), 255);
            int yVal = (int)Mathf.Min(Mathf.Max(yRaw / 2f, 1), 255);

            newTex.SetPixel(xVal, yVal, Color.red);
            newTex.SetPixel(xVal + 1, yVal, Color.red);
            newTex.SetPixel(xVal - 1, yVal, Color.red);
            newTex.SetPixel(xVal, yVal + 1, Color.red);
            newTex.SetPixel(xVal, yVal - 1, Color.red);
            newTex.SetPixel(xVal + 1, yVal + 1, Color.red);
            newTex.SetPixel(xVal - 1, yVal - 1, Color.red);
            newTex.SetPixel(xVal + 1, yVal - 1, Color.red);
            newTex.SetPixel(xVal - 1, yVal + 1, Color.red);
        }
        newTex.Apply();

    }

    public void plotPoint(Vector2 point)
    {
        Texture2D newTex = GetComponent<RawImage>().texture as Texture2D;
        GetComponent<RawImage>().texture = newTex;

        float xRaw = point.x;
        float yRaw = point.y;

        int xVal = (int)Mathf.Min(Mathf.Max(xRaw / 4.8f, 0), 256);
        int yVal = (int)Mathf.Min(Mathf.Max(yRaw / 2.5f, 0), 256);

        newTex.SetPixel(xVal, yVal, Color.red);
        newTex.Apply();
    }

    public void plotVerticalLine(float xRaw)
    {
        Texture2D newTex = GetComponent<RawImage>().texture as Texture2D;
        GetComponent<RawImage>().texture = newTex;

        int xVal = (int)Mathf.Min(Mathf.Max(xRaw / 4.8f, 0), 256);
        for(int i = 0; i < 256; i++)
        {
            newTex.SetPixel(xVal, i, Color.cyan);
        }
        
        newTex.Apply();
    }
	
	// Update is called once per frame
	void Update () {

	}
}
