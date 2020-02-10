using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeGenScript : MonoBehaviour {

    public GameObject beePrefab;
    
    public GameObject[] bee;
    public int nBees = 200;

    public float spacing = .2f;

    public GameObject queen;
    float ph = 0;

    private void Awake()
    {
        BB.bees = this;
    }
    // Start is called before the first frame update
    void Start() {
        //declare at BlackBoard

        float tx, ty;

        bee = new GameObject[nBees];

        for (int i = 0; i < nBees; i++) {
            tx = Random.Range(0.1f, 10f);
            ty = Random.Range(0.1f, 10f);
            bee[i] = Instantiate(beePrefab, new Vector3(tx, 2, ty), Quaternion.identity);
        }     

        

    }

    void FixedUpdate() {
        //move Queen Bee
        ph += .005f;
        queen.transform.position = new Vector3(5f+ Mathf.Sin(ph * 3), 1.2f+Mathf.Sin(ph * 4), 5f+Mathf.Sin(ph * 5)) ;


    }



}
