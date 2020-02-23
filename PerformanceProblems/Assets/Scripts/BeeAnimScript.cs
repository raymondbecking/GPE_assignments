using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeeAnimScript : MonoBehaviour {

    GameObject[] bee;
    float dist = 0;
    Vector3 middle = new Vector3(5f, 2f, 5f);
    int counter = 0;
    Rigidbody beeRigid;

    private void Start()
    {
        beeRigid = GetComponent<Rigidbody>(); 
    }

    void FixedUpdate() {


        //grab array
        bee = BB.bees.bee;

        if (counter < BB.bees.nBees - 1)
        {
            counter++;
        }
        else
        {
            counter = 0;
        }

        // 1st, go to ONE point
        Vector3 oink = Vector3.ClampMagnitude(middle - transform.position, 1);
        beeRigid.AddForce(oink * 2f, ForceMode.Acceleration);

        //for (int i = 0; i < BB.bees.nBees; i++) {
        //    //do not check self!
            if (bee[counter] != this.gameObject) {
                //distance check:
                dist = Vector3.Distance(transform.position, bee[counter].transform.position);
                if (dist < BB.bees.spacing) {
                    Vector3 targetDirection = Vector3.ClampMagnitude(transform.position - bee[counter].transform.position, 1);
                    beeRigid.AddForce(targetDirection * 6f, ForceMode.Acceleration);
                }
            }
        //}
        //finally, avoid Queen Bee
        dist = Vector3.Distance(transform.position, BB.bees.queen.transform.position);
        if (dist < BB.bees.spacing*2.5f) {
            Vector3 targetDirection = Vector3.ClampMagnitude(transform.position - BB.bees.queen.transform.position, 1);
            beeRigid.AddForce(targetDirection * 3f, ForceMode.Acceleration);
        }
    }

            
}
