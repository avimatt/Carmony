using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Map object to hold data for game in the case of multiple maps
public class Map : MonoBehaviour {

    public string name;
    public GameObject playerTopStart;
    public GameObject playerBottomStart;

    public List<Vector3> randomPowerupLocations;
    public GameObject checkpointSystem;
    public GameObject rocketStopSystem;
    public List<GameObject> fireworkList;

    public int numLaps;

    public Sprite miniMapImage;
    public int x;
    public int z;

    public float mapYAirModifier;
    public float mapYGroundModifier;

    public float topMapYAirModifier;
    public float bottomMapYAirModifier;

    // Use this for initialization
    void Start () {
        topMapYAirModifier = mapYAirModifier;
        bottomMapYAirModifier = mapYAirModifier;
	}
	
	// Update is called once per frame
	void Update () {
	    if (Main.S.carTop.GetComponent<ArcadeVehicle>().grounded && Time.time - Main.S.carTop.GetComponent<UserInteraction>().bombTimer > .5f)
        {
            topMapYAirModifier = mapYAirModifier;
        }
        if (Main.S.carBottom.GetComponent<ArcadeVehicle>().grounded && Time.time - Main.S.carBottom.GetComponent<UserInteraction>().bombTimer > .5f)
        {
            bottomMapYAirModifier = mapYAirModifier;
        }
    }
}
