using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityStandardAssets.Vehicles.Car;

public enum powerUpType
{
	speed,
	letters,
	swap,
    random,
    oil,
	portal,
    empty
}

public class PowerUp : MonoBehaviour
{

    List<string> xboxLetters = new List<string>(); // List of XBox controller letters
    
    public powerUpType type;
    public bool isRandom;
    // Use this for initialization
    void Start()
    {
        xboxLetters.Add("X");
        xboxLetters.Add("Y");
        xboxLetters.Add("B");
        xboxLetters.Add("A");

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newRot = gameObject.GetComponent<Transform>().rotation.eulerAngles;
        newRot.y += 1.5f;
        newRot.y = newRot.y % 360;
        gameObject.transform.rotation = Quaternion.Euler(newRot);
    }

	// When Player has collided with the power up
    void OnTriggerEnter(Collider coll)
    {

		print("triggerd");
		// Remove it from the scene
        
		// Determine which car hit it

        // TODO: WHY THE FUCK do we use isTop in some places, and isBottom in others? We need to be consistent.
        bool isBottomScreen = coll.GetComponentInParent<Transform>().GetComponentInParent<UserInteraction>().isCarBottom;
        if (!isBottomScreen)
        {
            Main.S.carTop.GetComponent<CarState>().powerupsHit++;
        }
        else
        {
            Main.S.carBottom.GetComponent<CarState>().powerupsHit++;
            
        }
        Logger.S.writeFile(!isBottomScreen, "Picked Up Powerup at " + Main.S.getGameTime());

        if (type == powerUpType.random)
        {
            bool isBehind = CarState.isCarBehind(!isBottomScreen);
            if (isBehind)
            {
                int randInt = Random.Range(0, 10);
                if (randInt < 5)
                    type = powerUpType.swap;
				else if(randInt < 8)
					type = powerUpType.portal;
                else if (randInt == 8)
                    type = powerUpType.oil;
                else
                    type = powerUpType.speed;
            }
            else
            {
                int randInt = Random.Range(0, 10);
                print(randInt);
                if (randInt < 3)
                {
                    type = powerUpType.speed;
                }
                else if (randInt < 7)
                {
                    type = powerUpType.oil;
                }
                else
                    type = powerUpType.swap;
            }
        }

        if ((isBottomScreen && Main.S.carBottomDone) || (!isBottomScreen && Main.S.carTopDone))
        {
            print("cant pick up item now");
        }
        else
        {
            CarmonyGUI.S.GiveTeamPowerup(!isBottomScreen, type);
        }
        if (!isRandom)
            Destroy(gameObject);
        else
            StartCoroutine("destroyObject");

    }

    IEnumerator destroyObject()
    {
        print("destroying object");
        gameObject.GetComponent<MeshRenderer>().enabled = false;
        gameObject.GetComponent<BoxCollider>().enabled = false;
        type = powerUpType.random;
        yield return new WaitForSeconds(4f);
        gameObject.GetComponent<MeshRenderer>().enabled = true;
        gameObject.GetComponent<BoxCollider>().enabled = true;
        print("reactive");
    }

    // Generate random list of 4 letters
    List<string> getNewLetterList()
    {
        List<string> letterList = new List<string>();
        int numLetters = 4;
        for (int i = 0; i < numLetters; i++)
        {
            int randInt = Random.Range(0, 4);
            letterList.Add(xboxLetters[randInt]);
        }
        return letterList;
    }

	// Carry out the power ups action (To be called after the player has inputed the full sequence)
	public static void ActivatePowerUp(bool topPlayer, powerUpType type){

        if (type == powerUpType.speed) {
			if (topPlayer) {
				Main.S.carTop.GetComponent<UserInteraction> ().startBoost ();
			} else {
				Main.S.carBottom.GetComponent<UserInteraction> ().startBoost ();
			}
			PowerupGenerator.S.numInstantiatedSpeed--;
		} else if (type == powerUpType.swap) {
			if (topPlayer) {
				Main.S.carBottom.GetComponent<CarUserControl> ().playerSwap ();
			} else {
				Main.S.carTop.GetComponent<CarUserControl> ().playerSwap ();
			}
			PowerupGenerator.S.numInstantiatedSwap--;
		} else if (type == powerUpType.oil) {
			if (topPlayer) {
				Main.S.carTop.GetComponent<UserInteraction> ().placeOilSpill (); 
			} else {
				Main.S.carBottom.GetComponent<UserInteraction> ().placeOilSpill ();
			}
		} else if (type == powerUpType.portal) {
			if (topPlayer) {
				Main.S.carTop.GetComponent<UserInteraction> ().moveToNextCheckpoint (); 
			} else {
				Main.S.carBottom.GetComponent<UserInteraction> ().moveToNextCheckpoint ();
			}
		}
        Logger.S.writeFile(topPlayer, "Activated Powerup " + type + " at: " + Main.S.getGameTime());
        if (topPlayer)
            Main.S.carTop.GetComponent<CarState>().powerupsActivated++;
        else
            Main.S.carBottom.GetComponent<CarState>().powerupsActivated++;

    }

}
