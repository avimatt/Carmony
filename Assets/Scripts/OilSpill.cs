using UnityEngine;
using System.Collections;

public class OilSpill : MonoBehaviour {

    // Use this for initialization

    float startime;
	void Start () {
        startime = Time.time;
        StartCoroutine("endOilSlickPhysical");
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    // When Player has collided with the power up
    void OnTriggerEnter(Collider coll)
    {
        if (Time.time - startime < .2)
            return;
        print("triggerd");
        //Check if it is a car that enters the checkpoint
        Transform playerTrans = null;
        Transform tmp = coll.transform.parent;
        if (tmp)
        {
            playerTrans = tmp.transform.parent;
        }
        else
        {
            return;
        }

        if (playerTrans && playerTrans.tag == "Player")
        {
            // Determine which car hit it
            bool isBottomScreen = coll.GetComponentInParent<Transform>().GetComponentInParent<UserInteraction>().isCarBottom;
            // Generate random activation sequence
            // Show the player the sequence
            WheelCollider[] colliderArray = coll.GetComponentInParent<UserInteraction>().GetComponentsInChildren<WheelCollider>();
            for (int i = 0; i < colliderArray.Length; i++)
            {
                WheelFrictionCurve newCurve = colliderArray[i].sidewaysFriction;
                newCurve.extremumSlip = 1;
                newCurve.asymptoteSlip = 1;
                newCurve.extremumValue = .2f;
                colliderArray[i].sidewaysFriction = newCurve;
            }
            StartCoroutine("endOilSlickCause", colliderArray);
        }

    }
    IEnumerator endOilSlickPhysical()
    {
        yield return new WaitForSeconds(15);
        Destroy(gameObject);
    }
    IEnumerator endOilSlickCause(WheelCollider[] colliderArray)
    {
        yield return new WaitForSeconds(5);
        for (int i = 0; i < colliderArray.Length; i++)
        {
            print("undoing");
            WheelFrictionCurve newCurve = colliderArray[i].sidewaysFriction;
            newCurve.extremumSlip = .2f;
            newCurve.asymptoteSlip = .5f;
            newCurve.extremumValue = 1f;
            colliderArray[i].sidewaysFriction = newCurve;
        }
    }
}
