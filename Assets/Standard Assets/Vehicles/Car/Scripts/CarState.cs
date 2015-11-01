using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarState : MonoBehaviour {
    // Array of checkpoints in the track
    public List<Transform> checkpoints;
    public int currCheckpoint = 0;
    public int currLap = 0;
	// Use this for initialization
	void Start () {
        // Populate the checkpoints array with all the checkpoints
        // Iterates through the children of the Checkpoints object and adds
        // them to the checkpoints list.
        foreach (Transform child in GameObject.Find("Checkpoints").transform)
        {
            checkpoints.Add(child);
        }
        // First checkpoint is start
        this.currCheckpoint = 0;
        // Start laps at 0
        this.currLap = 0;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
