using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Make sure to change the class name (CCSphere) to whatever you called your script.
public class ChangeColor : MonoBehaviour
{

    public Material[] materials;
    //Allows input of material colors in a set size of array;
    public Renderer Rend;
    //What are we rendering? Input object(Sphere,Cylinder,...) to render.

    private int index = 1;
    //Initialize at 1, otherwise you have to press the ball twice to change colors at first.

    // Use this for initialization
    void Start ()
    {
        Rend = GetComponent<Renderer> ();//Gives functionality for the renderer
        if (Rend == null) {
            Rend = GetComponentInChildren<Renderer> ();
        }
        Rend.enabled = true;//Makes the rendered 3d object visable if enabled;
    }

    void OnMouseDrag ()
    {

        if (materials.Length == 0)//If there are no materials nothing happens.
            return;


        index += 1;//When mouse is pressed down we increment up to the next index location

        if (index == materials.Length + 1)//When it reaches the end of the materials it starts over.
                index = 1;

        print (index);//used for debugging

        Rend.sharedMaterial = materials [index - 1]; //This sets the material color values inside the index  
    }

    void OnMouseUp ()
    {
        Rend.sharedMaterial = materials [0];
    }
}