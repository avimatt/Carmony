using UnityEngine;
using System.Collections;

public class OilSpill : MonoBehaviour {

    // Use this for initialization

    float startime;
    public bool isFromTop;
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
			ArcadeVehicle vehicle = coll.GetComponentInParent<Transform>().GetComponentInParent<ArcadeVehicle>();
            bool isBottomScreen = vehicle.isBottomCar;
            if (isBottomScreen && !isFromTop)
            {
                return;
            }
			else if (!isBottomScreen && isFromTop)
            {
                return;
            }
            // create oil slick effect
			vehicle.horizontalFriction = .01f;
            StartCoroutine("endOilSlickCause", vehicle);
        }

    }
    IEnumerator endOilSlickPhysical()
    {
        yield return new WaitForSeconds(75);
        Destroy(gameObject);
    }

    IEnumerator endOilSlickCause(ArcadeVehicle vehicle)
    {
        yield return new WaitForSeconds(5);
        // undo oil slick effect
		vehicle.horizontalFriction = .5f;
    }
}
