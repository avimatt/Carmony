using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}

    void OnTriggerEnter(Collider other)
    {
        print("Hit checkpoint");
        //Check if it is a car that enters the checkpoint
        Transform playerTrans = null;
        Transform tmp = other.transform.parent;
        if (tmp) {
            playerTrans = tmp.transform.parent;
        }
        if (playerTrans && playerTrans.tag=="Player") {
            print("player hit checkpoint");
            // other is player
            // Get a reference to the CarState script
            CarState player = playerTrans.GetComponent<CarState>();
            // Is this checkpoint the same as the next checkpoint of the player
            if (transform == player.checkpoints[player.currCheckpoint].transform)
            {
                // increment the checkpoint to the next one
                // Don't go past the end of checkpoint array
                if (player.currCheckpoint + 1 < player.checkpoints.Count)
                {
                    // If they pass the starting line (first checkpoint), increment lap count
                    if (player.currCheckpoint == 0)
                    {
                        player.currLap++;
                    }
                    player.currCheckpoint++;
                }
                else
                {
                    // Out of checkpoints, go back to the first
                    player.currCheckpoint = 0;
                }
            }
        } else {
            // if not the player, don't continue
            return;
        }      
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
