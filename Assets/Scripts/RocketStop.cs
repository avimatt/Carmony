using UnityEngine;
using System.Collections;

public class RocketStop : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        // If a rocket hit this RocketStop:
        Transform rocketTrans = other.transform.parent;
        if (rocketTrans && rocketTrans.tag == "Rocket")
        {
            // It's a rocket
            // Get a reference to the Rocket script
            Rocket rocket = rocketTrans.GetComponent<Rocket>();
            // get the rocket moving to the next checkpoint
            rocket.SetRocketStopTrajectory(rocket.getNextRocketStop());
            // increment the rocketstop to the next one, don't go past the end of th rocketStops array
            rocket.incrRocketStop();

        }
        
        // If not a rocket, see if it's a car
        else
        {
            Transform playerTrans = null;
            Transform tmp = other.transform.parent;
            if (!tmp)
            {
                // This means it's not a car, so stop
                return;
            }
            playerTrans = tmp.transform.parent;
            if (playerTrans && playerTrans.tag == "Player" && other.name == "ColliderBottom")
            {
                // Confirmed that it is a car at this point
                // Get a ref to the CarState script
                //CarState player = playerTrans.GetComponent<CarState>();
                // increment the rocketstop to the next one
                //player.incrRocketStop();
            }
            else
            {
                // If not the player, don't continue
                return;
            }
        }
    }

	// Update is called once per frame
	void Update () {
	
	}
}
