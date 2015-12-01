using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CarState : MonoBehaviour {
    // Array of checkpoints in the track
    public List<Transform> checkpoints;
    
    public List<Transform> topResetLocation;
    public List<Transform> bottomResetLocation;

    public int currCheckpoint = 0;
    public int currLap = 0;

    // Array of the RocketStops in the track
    public List<Transform> rocketStops;
    // currRocketStop is an index into the rocketStops array, indicating
    // which one this car has just passed.
    public int currRocketStop = 0;

    public bool perfectRace = true;
    public bool perfectLap = true;
    public bool perfectCheckpoint = true;

    public int numPerfectCheckpoints;
    public string totalTime;
    public int powerupsHit;
    public int powerupsActivated;
    public int resets;
    public string name;
    bool set;

	// Use this for initialization
	void Start () {

    }
	
	// Update is called once per frame
	void Update () {
        if (!set && Main.S.practicing)
        {
            setRocketStops();
            setCheckpoints();
        }
	}

    public int getNextRocketStop()
    {
        if (currRocketStop + 1 < rocketStops.Count)
            return currRocketStop + 1;
        else
            return 0;
    }

    public void incrRocketStop()
    {
        this.currRocketStop = getNextRocketStop();
    }

    void setRocketStops()
    {
        // Populate the array of rocketStops
        foreach (Transform child in Main.S.Map.GetComponent<Map>().rocketStopSystem.transform)
        {
            rocketStops.Add(child);
        }
        // Set first rocketstop
        this.currRocketStop = 0;
    }

    void setCheckpoints()
    {
        // Populate the checkpoints array with all the checkpoints
        // Iterates through the children of the Checkpoints object and adds
        // them to the checkpoints list.
        foreach (Transform child in Main.S.Map.GetComponent<Map>().checkpointSystem.transform)
        {
            checkpoints.Add(child);
            topResetLocation.Add(child.Find("Top").transform);
            bottomResetLocation.Add(child.Find("Bottom").transform);
        }
        // First checkpoint is start
        this.currCheckpoint = 0;
        // Start laps at 0
        this.currLap = 0;
        set = true;
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
