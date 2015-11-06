﻿using UnityEngine;
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

    //Determines whether the car passed it is behind by atleast a checkpoint in the race.
    //would like to do closer estimations but no closer meauserments than checkpoints
    //yes avi, i used a ternary.
    static public bool isCarBehind(bool isTop)
    {
        CarState topCarState = Main.S.carTop.GetComponent<CarState>();
        CarState bottomCarState = Main.S.carBottom.GetComponent<CarState>();
        int checkpointIndexTopAdjusted = topCarState.currCheckpoint == 0 ? topCarState.checkpoints.Count : topCarState.currCheckpoint;
        int checkpointIndexBottomAdjusted = bottomCarState.currCheckpoint == 0 ? bottomCarState.checkpoints.Count : bottomCarState.currCheckpoint;
        int topTotal = topCarState.currLap * topCarState.checkpoints.Count + checkpointIndexTopAdjusted;
        int bottomTotal = bottomCarState.currLap * bottomCarState.checkpoints.Count + checkpointIndexBottomAdjusted;
        if (isTop && bottomTotal > topTotal)
            return true;
        else if (!isTop && topTotal > bottomTotal)
            return true;
        else
            return false;
    }
}
