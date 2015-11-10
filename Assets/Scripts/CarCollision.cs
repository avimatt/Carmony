using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;


public class CarCollision : MonoBehaviour {


    // Use this for initialization
    void Start () {

    }

    // Update is called once per frame
    void Update () {
	
	}


    void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.layer == 0)
        {
            coll.gameObject.GetComponentInParent<CarState>().perfectRace = false;
            coll.gameObject.gameObject.GetComponentInParent<CarState>().perfectLap = false;
            coll.gameObject.gameObject.GetComponentInParent<CarState>().perfectCheckpoint = false;

            printCollisionData(coll);

        }
    }

    void printCollisionData(Collision coll)
    {
        bool isBottomCar = coll.gameObject.gameObject.GetComponentInParent<CarUserControl>().isBottomCar;
        string carStr = isBottomCar ? "Bottom Car" : "Top Car";
        float xPos = coll.gameObject.gameObject.GetComponentInParent<Transform>().position.x;
        float zPos = coll.gameObject.gameObject.GetComponentInParent<Transform>().position.z;
        print(carStr + " hit at x=" + xPos + ",z=" + zPos + "   At " + Main.S.getGameTime());
        if (!isBottomCar)
            Logger.S.writeFile(true,carStr + " hit at x=" + xPos + ",z=" + zPos + "   At " + Main.S.getGameTime());
        else
            Logger.S.writeFile(false, carStr + " hit at x=" + xPos + ",z=" + zPos + "   At " + Main.S.getGameTime());

    }
}
