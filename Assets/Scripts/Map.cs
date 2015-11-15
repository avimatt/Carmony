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
    public List<GameObject> fireworkList;

    public int numLaps;

    public Sprite miniMapImage;
    public int x;
    public int z;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
