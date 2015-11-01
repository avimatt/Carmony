//tweak the direction of the swapArrows and put in the speed up

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PowerupGenerator : MonoBehaviour {
	public static PowerupGenerator S;
	
	public bool[] instantiatedSwap, instantiatedSpeed;
	
	public GameObject SwapArrow, Lightning;
	private GameObject obj;
	
	public int numPowerups;
	private int rand, randX, randZ, curSwapLocation;
	public int numInstantiatedSwap, numInstantiatedSpeed;
	
	public Vector3[] swapGeneration;
	private Vector3 turn90, tempLocation;
		// Use this for initialization
	void Start () {
		//init values
		S = this;
		numInstantiatedSwap = 0;
		numInstantiatedSpeed = 0;
		curSwapLocation = 0;
		turn90 = new Vector3(0,90,0);
		swapGeneration = new Vector3[6];
		instantiatedSwap = new bool[numPowerups];
		instantiatedSpeed = new bool[numPowerups];
		for(int i = 0; i < instantiatedSwap.Length; ++i){
			instantiatedSwap[i] = false;
			instantiatedSpeed[i] = false;
		}
		swapGeneration[0] = new Vector3(162,2,319);
		swapGeneration[1] = new Vector3(121,2,241);
		swapGeneration[2] = new Vector3(175,2,152);
		swapGeneration[3] = new Vector3(308,2,394);
		swapGeneration[4] = new Vector3(308,2,455);
		swapGeneration[5] = new Vector3(381,2,334);
		
		//end init
		for(int i = 0; i < numPowerups; ++i){
			//swap powerUps start
			createSwap();
			//swap powerUps end
			
			//speed powerUps start
			createSpeed();
			//speed powerUps end
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(numInstantiatedSpeed < numPowerups){
			createSpeed();
		}
		if(numInstantiatedSwap < numPowerups){
			createSwap();
		}
	}
	void createSwap(){
		if(curSwapLocation == 4 || curSwapLocation == 1){
			obj = Instantiate(SwapArrow, swapGeneration[curSwapLocation], Quaternion.identity) as GameObject;
			obj.transform.Rotate(turn90);
		} else {
			obj = Instantiate(SwapArrow, swapGeneration[rand], Quaternion.identity) as GameObject;
		}
		
		++numInstantiatedSwap;
		++curSwapLocation;
		if(curSwapLocation == 5)
			curSwapLocation = 0;
		
	}
	void createSpeed(){
		rand = UnityEngine.Random.Range(0, 1); 	//choose between 5 location area to place
		if(rand == 1) {
			randX = UnityEngine.Random.Range(100,230);
			randZ = UnityEngine.Random.Range(447,462);
		} else {
			randX = UnityEngine.Random.Range(160,290);
			randZ = UnityEngine.Random.Range(50,57);
		}
		tempLocation = new Vector3(randX,2,randZ);
		obj = Instantiate(Lightning,tempLocation, Quaternion.identity) as GameObject;
		++numInstantiatedSpeed;
	}
	
}
