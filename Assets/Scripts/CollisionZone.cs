using UnityEngine;
using System.Collections;
using System;

public class CollisionZone : MonoBehaviour {

	bool bottomCar;
	bool enteredOnce;
	float timeEntered;
	string gameTimeEntered;

	void OnTriggerEnter(Collider other){
		if (!enteredOnce) {
			if (other.attachedRigidbody.gameObject.name == "VehicleTop") {
				topInCollisionZone = true;
				bottomCar = false;
			} else {
				bottomCar = true;
				bottomInCollisionZone = true;
			}

			timeEntered = Time.time;
			gameTimeEntered = Main.S.getGameTime();
			enteredOnce = true;
		}
	}
	
	void OnTriggerExit(Collider other) {
		if((other.attachedRigidbody.gameObject.name == "VehicleTop" && !bottomCar) || (other.attachedRigidbody.gameObject.name == "VehicleBottom" && bottomCar)){

			if(bottomCar){
				bottomInCollisionZone = false;
			} else {
				topInCollisionZone = false;
			}
			printCollisionData();
			Destroy(gameObject);
		}
	}

	// Use this for initialization
	void Start () {
		enteredOnce = false;
	}

	bool topInCollisionZone {
		get { return Main.S.topInCollisionZone; }
		set { Main.S.topInCollisionZone = value; }
	}

	bool bottomInCollisionZone {
		get { return Main.S.bottomInCollisionZone; }
		set { Main.S.bottomInCollisionZone = value; }
	}

	void printCollisionData()
	{
		string carStr = bottomCar ? "Bottom Car" : "Top Car";
		float xPos = gameObject.transform.position.x;
		float zPos = gameObject.transform.position.z;
		if (!bottomCar) {
			Logger.S.writeFile (true, carStr + " hit wall at x = " + xPos + ",z = " + zPos + "   At " + gameTimeEntered);
			Logger.S.writeFile (true, carStr + " was stuck there for " + TimeSpan.FromSeconds (Time.time - timeEntered).ToString ());
		} else {
			Logger.S.writeFile (false, carStr + " hit wall at x = " + xPos + ",z = " + zPos + "   At " + gameTimeEntered);
			Logger.S.writeFile (false, carStr + " was stuck there for " + TimeSpan.FromSeconds (Time.time - timeEntered).ToString ());
		}
	}
}
