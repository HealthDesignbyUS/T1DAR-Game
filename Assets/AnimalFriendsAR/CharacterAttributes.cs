using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

//using NUnit.Framework;

public enum characterType
{
    cat,
    dog,
    owl,
    turtle
}

public class CharacterAttributes : MonoBehaviour
{

    static public CharacterAttributes S;
    public float bloodSugarLevel = 120;

    public GameObject graphObj;

    public Glucodyn glucodyn = new Glucodyn ();

    public Animation anim;

    public int foodEaten;
    public int sugarTaken;
    public int insulinTaken;
    public int meterChecked;
    public bool moved;
    public bool moving;

    private IEnumerator coroutine;

    public Vector3 destPos;
    public Quaternion destRot;

    public characterType charType;

    public int currentScore;

    public float startTime;
    public float curTimeTick;

    void Awake ()
    {
        S = this;
        anim = GetComponent<Animation> ();
    }

    // Use this for initialization
    void Start ()
    {
        startTime = Time.time;
        destPos = transform.position;
        //bloodSugarLevel = 120;
        
        //glucodyn.AddCarbs (5f, 60f);
        //glucodyn.AddInsulin(0f, 150f);

        updateLineGraph ();
        StartCoroutine("updateBloodSugar", 7.2f);

        print (charType);
    }
	
    // Update is called once per frame
    void Update ()
    {
        //bloodSugarLevel = (float)glucoseEvents.Values.Last();


        //print(glucoseEvents.Keys);
        
        bloodSugarLevel = Mathf.Max (35, bloodSugarLevel);

        //print (charType);

        if (bloodSugarLevel < 40) {
            switch (charType) {
            case characterType.cat:
                anim.Play ("Sleep Cat");
                break;
            case characterType.dog:
                anim.Play ("Sleep Dog");
                break;
            case characterType.owl:
                anim.Play ("Sleep Owl");
                break;
            case characterType.turtle:
                anim.Play ("Sleep Turtle");
                break;
            }
            return;
        } else if (bloodSugarLevel > 300) {
            switch (charType) {
            case characterType.cat:
                anim.Play ("Run Cat");
                break;
            case characterType.dog:
                anim.Play ("Run Dog");
                break;
            case characterType.owl:
                anim.Play ("Run Owl");
                break;
            case characterType.turtle:
                anim.Play ("Run Turtle");
                break;
            }
        } else {
            if (!moving) {
                switch (charType) {
                case characterType.cat:
                    anim.Play ("Idle Cat");
                    break;
                case characterType.dog:
                    anim.Play ("Idle Dog");
                    break;
                case characterType.owl:
                    anim.Play ("Idle Owl");
                    break;
                case characterType.turtle:
                    anim.Play ("Idle Turtle");
                    break;
                }
            }
        }

        if(LevelTemplate.S.curLevel == 2) //disable char movement for level 2
        {
            return;
        }

        if (transform.position != destPos) {
            moving = true;
            if (LevelTemplate.S.curPhase == 1) {
                moved = true;
            }
            if (bloodSugarLevel <= 300) {
                switch (charType) {
                case characterType.cat:
                    anim.Play ("Walk Cat");
                    break;
                case characterType.dog:
                    anim.Play ("Walk Dog");
                    break;
                case characterType.owl:
                    anim.Play ("Walk Owl");
                    break;
                case characterType.turtle:
                    anim.Play ("Walk Turtle");
                    break;
                }
            }
            moveToPosition (destPos);
        } else {
            moving = false;
            if (transform.rotation != destRot) {
                rotateToDest (destRot);
            }
        }
    }

    public void updateLineGraph ()
    {
        var glucoseEvents = glucodyn.GetGlucoseEvents ();

        var points = new Vector2[glucoseEvents.Values.Count ()];

        int iter = 0;

        foreach (var glucoseEvent in glucoseEvents) {
            //Debug.Log (glucoseEvent);
            //points[iter] = new Vector3(glucoseEvent.Key * 0.05f + transform.position.x, (float)glucoseEvent.Value * 0.05f + transform.position.y, transform.position.z + 1);
            points [iter] = new Vector2 (glucoseEvent.Key, (float)glucoseEvent.Value);
            iter++;
        }

        graphObj.GetComponent<graphDisp> ().plotGraph (points);
        graphObj.GetComponent<graphDisp>().plotVerticalLine(curTimeTick);
        //line.GetComponent<LineRenderer>().numPositions = glucoseEvents.Values.Count();
        //line.GetComponent<LineRenderer>().SetPositions(points);
    }

    public void moveToPosition (Vector3 pos)
    {
        float step = 0.8f * Time.deltaTime;
        transform.position = Vector3.MoveTowards (transform.position, pos, step);
//        transform.rotation = Quaternion.LookRotation(Vector3.RotateTowards(transform.forward, pos, step, 0.0f));
        transform.LookAt (destPos);
    }

    public void rotateToDest (Quaternion rot)
    {
        float step = 90f * Time.deltaTime;
        transform.rotation = Quaternion.RotateTowards (transform.rotation, rot, step);
    }

    public void changeBS (float amount, float interval)
    {
        coroutine = changeBSlevelOverTime (amount, interval);
        StartCoroutine (coroutine);
    }

    public IEnumerator changeBSlevelOverTime (float amount, float interval)
    {
        float amtChange = amount / interval;
        while (interval > 0) {
            bloodSugarLevel += amtChange;
            if (bloodSugarLevel < 0f) {
                bloodSugarLevel = 0f;
                break;
            }
            interval -= 1f;
            yield return new WaitForSeconds (1f);
        }
    }

    public void addPoints (int pts)
    {
        currentScore += pts;
    }

    IEnumerator updateBloodSugar(float time)
    {
        while (true)
        {
            updateBS();
            //graphObj.GetComponent<graphDisp>().plotVerticalLine(curTimeTick);
            //print(curTimeTick);
            yield return new WaitForSeconds(time);
        }

    }

    public void updateBS()
    {
        var glucoseEvents = glucodyn.GetGlucoseEvents();
        float newTimeTick = (float)(Mathf.Min(((int)((Time.time - startTime) / (7.2f))), 149) * 7.2f);
        bloodSugarLevel = (float)glucoseEvents[curTimeTick];
        if (curTimeTick != newTimeTick)
        {
            curTimeTick = newTimeTick;
            updateLineGraph();
        }
    }
}
