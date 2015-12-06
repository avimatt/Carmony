using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;

public enum powerUpType
{
	speed,
	letters,
	swap,
    random,
    oil,
	portal,
    empty,
    rocket
}

public class PowerUp : MonoBehaviour
{

    private MeshRenderer 		m_meshRenderer;
    private BoxCollider 		m_boxCollider;
    public GameObject 			m_partialSystem;

    public powerUpType 			type;
    public bool 				isRandom;

    // Use this for initialization
    void Start()
    {
        m_meshRenderer = gameObject.GetComponent<MeshRenderer>();
        m_boxCollider = gameObject.GetComponent<BoxCollider>();
        if (gameObject.GetComponentInChildren<ParticleSystem>())
        {
            m_partialSystem = gameObject.GetComponentInChildren<ParticleSystem>().gameObject;
        }
        if (m_partialSystem)
            m_partialSystem.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 newRot = gameObject.GetComponent<Transform>().rotation.eulerAngles;
        newRot.y += 1.5f;
        newRot.y = newRot.y % 360;
        gameObject.transform.rotation = Quaternion.Euler(newRot);
    }

	/// <summary>
	/// When Player has collided with the power up
	/// </summary>
    void OnTriggerEnter(Collider coll)
    {
        // The rocket is an object going through the track, and may run into a powerup
        // so we need to account for that and not take any actions if a powerup is hit
        // by a rocket.
        Transform rocketTrans = coll.transform.parent;
        if (rocketTrans && rocketTrans.tag == "Rocket")
        {
            // if it is a rocket, don't do anything
            return;
        }

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
                if (randInt < 3)
                {
                    type = powerUpType.rocket;
                }
                else if (randInt < 5)
                    type = powerUpType.swap;
				else if(randInt < 9)
					type = powerUpType.portal;
                else
                    type = powerUpType.speed;
            }
            else
            {
                int randInt = Random.Range(0, 10);

                if (randInt < 4)
                {
                    type = powerUpType.speed;
                }
                else if (randInt < 10)
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

	/// <summary>
	/// Coroutine to display powerup animation and respawn the powerup
	/// </summary>
    IEnumerator destroyObject()
    {
        m_meshRenderer.enabled = false;
        m_boxCollider.enabled = false;
        m_partialSystem.gameObject.SetActive(true);

        type = powerUpType.random;

        yield return new WaitForSeconds(1f);
        m_partialSystem.gameObject.SetActive(false);
        yield return new WaitForSeconds(3f);
        m_meshRenderer.enabled = true;
        m_boxCollider.enabled = true;
    }

	/// <summary>
	/// Activates the power up. [To be called after players both hold "a"]
	/// </summary>
	/// <param name="topPlayer">If set to <c>true</c> top player.</param>
	/// <param name="type">Type.</param>
	public static void ActivatePowerUp(bool topPlayer, powerUpType type){

        if (type == powerUpType.speed) {
            // Play the speedboost sound
            AudioSource speedSound = GameObject.Find("SpeedBoostSound").GetComponent<AudioSource>();
            speedSound.Play();
			if (topPlayer) {
				Main.S.carTop.GetComponent<UserInteraction> ().startBoost ();
			} else {
				Main.S.carBottom.GetComponent<UserInteraction> ().startBoost ();
			}
			PowerupGenerator.S.numInstantiatedSpeed--;
		} else if (type == powerUpType.swap) {
            // Play the portal sound
            AudioSource swapSound = GameObject.Find("SwapSound").GetComponent<AudioSource>();
            swapSound.Play();
			if (topPlayer) {
				Main.S.carBottom.GetComponent<ArcadeVehicle> ().playerSwap ();
			} else {
				Main.S.carTop.GetComponent<ArcadeVehicle> ().playerSwap ();
			}
			PowerupGenerator.S.numInstantiatedSwap--;
		} else if (type == powerUpType.oil) {
            // Play the portal sound
            AudioSource oilSound = GameObject.Find("OilSound").GetComponent<AudioSource>();
            oilSound.Play();
			if (topPlayer) {
				Main.S.carTop.GetComponent<UserInteraction> ().placeOilSpill (); 
			} else {
				Main.S.carBottom.GetComponent<UserInteraction> ().placeOilSpill ();
			}
		} else if (type == powerUpType.portal) {
            // Play the portal sound
            AudioSource portalSound = GameObject.Find("PortalSound").GetComponent<AudioSource>();
            portalSound.Play();
			if (topPlayer) {
				Main.S.carTop.GetComponent<UserInteraction> ().moveToNextCheckpoint (); 
			} else {
				Main.S.carBottom.GetComponent<UserInteraction> ().moveToNextCheckpoint ();
			}
        }
        else if (type == powerUpType.rocket)
        {
            AudioSource rocketSound = GameObject.Find("RocketSound").GetComponent<AudioSource>();
            rocketSound.Play();
            // Spawn a rocket and set it's target;
            if (topPlayer) {
                int rocketstop = Main.S.carTop.GetComponent<CarState>().currRocketStop;
                Vector3 carpos = Main.S.carTop.transform.position;
                Main.S.carTop.GetComponent<UserInteraction>().spawnRocket(rocketstop, carpos, Main.S.carBottom);
            } else {
                int rocketstop = Main.S.carBottom.GetComponent<CarState>().currRocketStop;
                Vector3 carpos = Main.S.carBottom.transform.position;
                Main.S.carBottom.GetComponent<UserInteraction>().spawnRocket(rocketstop, carpos, Main.S.carTop);
            }
        }
        Logger.S.writeFile(topPlayer, "Activated Powerup " + type + " at: " + Main.S.getGameTime());
        if (topPlayer)
            Main.S.carTop.GetComponent<CarState>().powerupsActivated++;
        else
            Main.S.carBottom.GetComponent<CarState>().powerupsActivated++;

    }

}
