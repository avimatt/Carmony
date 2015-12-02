using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Checkpoint : MonoBehaviour {

    float lastCheckpointTime;

	// Use this for initialization
	void Start () {
	
	}

    public void hitCheckpoint(Transform playerTrans)
    {
        print("player hit checkpoint");
        lastCheckpointTime = Time.time;
        // other is player
        // Get a reference to the CarState script
        CarState player = playerTrans.GetComponent<CarState>();
        UserInteraction user = playerTrans.GetComponent<UserInteraction>();
        // Is this checkpoint the same as the next checkpoint of the player
        if (transform == player.checkpoints[player.currCheckpoint].transform)
        {
            string perfectMessage = "";
            if (player.perfectCheckpoint && player.currLap != 0)
            {
                print("perfect checkpoint");
                player.numPerfectCheckpoints++;
                Logger.S.writeFile(!playerTrans.GetComponent<UserInteraction>().isCarBottom, "Perfect Checkpoint " + player.currCheckpoint + " at: " + Main.S.getGameTime());
                perfectMessage = "Perfect";
            }
            player.perfectCheckpoint = true;


            // increment the checkpoint to the next one
            // Don't go past the end of checkpoint array
            if (player.currCheckpoint + 1 < player.checkpoints.Count)
            {
                // If they pass the starting line (first checkpoint), increment lap count
                if (player.currCheckpoint == 0)
                {
                    if (player.perfectLap && player.currLap != 0)
                    {
                        print("perfect lap");
                        perfectMessage = "Perfect Lap";
                    }
                    player.currLap++;

                    if (player.currLap == (Main.S.Map.GetComponent<Map>().numLaps))
                    {
                        StartCoroutine("printFinalLap", playerTrans.GetComponent<UserInteraction>().isCarBottom);
                    }
                    // If finished last lap
                    if (player.currLap == (Main.S.Map.GetComponent<Map>().numLaps + 1))
                    {
                        if (player.perfectRace)
                        {
                            print("perfect race");
                            perfectMessage = "Perfect Race";
                        }
                        Logger.S.writeFile(!playerTrans.GetComponent<UserInteraction>().isCarBottom, "Finished Race at: " + Main.S.getGameTime());
                        if (!user.portalTransport)
                        {
                            Main.S.endGame(!playerTrans.GetComponent<UserInteraction>().isCarBottom);
                        }
                    }
                }
                player.currCheckpoint++;
            }
            else
            {
                // Out of checkpoints, go back to the first
                player.currCheckpoint = 0;
            }
            if (perfectMessage != "")
            {
                if (player.GetComponent<ArcadeVehicle>().isBottomCar)
                {
                    StartCoroutine("printPerfectBottom", perfectMessage);

                }
                else
                {
                    StartCoroutine("printPerfectTop", perfectMessage);
                }
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        print("hit checkpoint");
        //Check if it is a car that enters the checkpoint
        Transform playerTrans = null;
        Transform tmp = other.transform.parent;
        if (tmp) {
            playerTrans = tmp.transform.parent;
        }
        else
        {
            return;
        }

        if (playerTrans && playerTrans.tag=="Player") {// && Time.time -lastCheckpointTime  > .5f) {
            hitCheckpoint(playerTrans);


        } else {
            // if not the player, don't continue
            return;
        }      
    }

    IEnumerator printFinalLap(bool isBottom)
    {
        if (isBottom)
        {
            CarmonyGUI.S.bottomSwapText.SetActive(true);
            CarmonyGUI.S.bottomSwapText.GetComponent<Text>().text = "Final Lap";
        }
        else
        {
            CarmonyGUI.S.topSwapText.SetActive(true);
            CarmonyGUI.S.topSwapText.GetComponent<Text>().text = "Final Lap";
        }
        yield return new WaitForSeconds(2);
        if (isBottom)
        {
            CarmonyGUI.S.bottomSwapText.SetActive(false);
            CarmonyGUI.S.bottomSwapText.GetComponent<Text>().text = "SWAP";
        }
        else
        {
            CarmonyGUI.S.topSwapText.SetActive(false);
            CarmonyGUI.S.topSwapText.GetComponent<Text>().text = "SWAP";
        }
    }

    IEnumerator printPerfectTop(string message)
    {

        CarmonyGUI.S.topPerfect.SetActive(true);
        CarmonyGUI.S.topPerfect.GetComponent<Text>().text = message;

        yield return new WaitForSeconds(2f);

        CarmonyGUI.S.topPerfect.SetActive(false);

    }
    IEnumerator printPerfectBottom(string message)
    {

        CarmonyGUI.S.bottomPerfect.SetActive(true);
        CarmonyGUI.S.bottomPerfect.GetComponent<Text>().text = message;

        yield return new WaitForSeconds(2f);

        CarmonyGUI.S.bottomPerfect.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
	
	}
}
