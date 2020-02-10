using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PineGen : MonoBehaviour {

    public GameObject pinePrefab;
    GameObject[] pine;
    int nPines = 5000;

    public Vector3 center;

    // Start is called before the first frame update
    void Start() {
        float tx, ty;

        pine = new GameObject[nPines];

        for (int i = 0; i < nPines; i++) {
            tx = Random.Range(-4.9f, 4.9f);
            ty = Random.Range(-4.9f, 4.9f);
            pine[i] = Instantiate(pinePrefab, new Vector3(tx, 0, ty) + center, Quaternion.identity);
            pine[i].transform.localScale *= Random.Range(.5f, 1.5f);

        }     
    }

    
}
