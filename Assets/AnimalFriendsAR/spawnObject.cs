using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnObject : MonoBehaviour
{
    public static spawnObject S;
    public Vector3 size;
    public GameObject[] Items;
    public int numObjects;
    public int prevPhase = -1;
    public List<GameObject> createdObj;

    public long timer = 0;

    void Awake ()
    {
        S = this;
        numObjects = 0;
        createdObj = new List<GameObject> ();
    }

    void Start ()
    {
        Spawn ();
    }
	
    // Update is called once per frame
    void Update ()
    {
        int mod = 250;
        if (numObjects <= 5) {
            mod = 30;
        } else if (numObjects <= 10) {
            mod = 75;
        }

        if (LevelTemplate.S.curLevel == 1) {
            if (LevelTemplate.S.curPhase != prevPhase) {
                timer = 0;
                if (LevelTemplate.S.curPhase == 3 || LevelTemplate.S.curPhase == 5 || LevelTemplate.S.curPhase == 7) {
                    for (int i = 0; i < createdObj.Count; ++i) {
                        GameObject g = createdObj [i];
                        //                    createdObj.Remove (g);
                        if (g) {
                            numObjects--;
                            Destroy (g);
                        }
                    }
                }
                prevPhase = LevelTemplate.S.curPhase;
                Spawn ();
            }
        } else if (LevelTemplate.S.curLevel == 2) {
            if (LevelTemplate.S.curPhase != prevPhase) {
                timer = 0;
                if (LevelTemplate.S.curPhase == 3) {
                    for (int i = 0; i < createdObj.Count; ++i) {
                        GameObject g = createdObj [i];
                        if (g && !g.GetComponent<Food> ().onTray) {
                            numObjects--;
                            Destroy (g);
                        }
                    }
                }
                prevPhase = LevelTemplate.S.curPhase;
                Spawn ();
            }
        }

        if ((timer % mod) == 0) {
            Spawn ();
        }
        timer++;
    }

    public void Spawn ()
    {
        int foodRand = 0;

        if (LevelTemplate.S.curLevel == 1) {
            if (LevelTemplate.S.curPhase <= 2) {
                foodRand = Random.Range (0, 2);
            } else if (LevelTemplate.S.curPhase <= 4) {
                foodRand = 2;
            } else if (LevelTemplate.S.curPhase <= 6) {
                foodRand = 3;
            } else {
                foodRand = Random.Range (0, 4);
            }
        } else if (LevelTemplate.S.curLevel == 2) {
            if (LevelTemplate.S.curPhase < 3) {
                foodRand = Random.Range (0, 5);
            } else {
                foodRand = 5;
            }
        }
            
        while (true) {
            Vector3 pos = transform.position + (Random.insideUnitSphere * 2.5f);
            Vector3 colliderSize = Items [foodRand].GetComponent<BoxCollider> ().size;
            Vector3 boxHalfSize = Vector3.Scale (Items [foodRand].transform.localScale, colliderSize) / 2;

            if (Physics.OverlapBox (pos, boxHalfSize, Items [foodRand].transform.rotation).Length == 0) {
                createdObj.Add (Instantiate (Items [foodRand], pos, Items [foodRand].transform.rotation));
                numObjects++;
                return;
            }
            print ("failed");
        }
    }
}
