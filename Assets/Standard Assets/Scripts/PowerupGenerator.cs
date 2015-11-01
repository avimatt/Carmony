//tweak the direction of the swapArrows and put in the speed up

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PowerupGenerator : MonoBehaviour {
	public static PowerupGenerator S;
	public Vector3[] swapGeneration;
	private Vector3 turn90, tempLocation;
	public bool[] instantiatedSwap, instantiatedSpeed;
	public int numPowerups;
	private int rand, randX, randZ, numInstantiatedSwap, numInstantiatedSpeed;
	private GameObject obj;
	public GameObject SwapArrow, Lightning;
	// Use this for initialization
	public GameObject[] swapVec, speedVec;
	void Start () {
		//init values
		S = this;
		numInstantiatedSwap = 0;
		numInstantiatedSpeed = 0;
		turn90 = new Vector3(0,90,0);
		swapGeneration = new Vector3[6];
		instantiatedSwap = new bool[numPowerups];
		instantiatedSpeed = new bool[numPowerups];
		speedVec = new GameObject[numPowerups];
		swapVec = new GameObject[numPowerups];
		for(int i = 0; i < instantiatedSwap.Length; ++i){
			instantiatedSwap[i] = false;
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
		do{
			rand = UnityEngine.Random.Range(0, swapGeneration.Length);
			
		}while(instantiatedSwap[rand]);
		instantiatedSwap[rand] = true;
		if(rand == 4 || rand == 1){
			obj = Instantiate(SwapArrow, swapGeneration[rand], Quaternion.identity) as GameObject;
			obj.transform.Rotate(turn90);
		}
		else
			obj = Instantiate(SwapArrow, swapGeneration[rand], Quaternion.identity) as GameObject;
		swapVec[rand] = obj;
		++numInstantiatedSwap;
		
	}
	void createSpeed(){
		rand = UnityEngine.Random.Range(0, 1); 	//choose between 5 location area to place
		
		if(rand == 1) {
			randX = UnityEngine.Random.Range(100,230);
			randZ = UnityEngine.Random.Range(447,462);
		}
		else {
			randX = UnityEngine.Random.Range(160,290);
			randZ = UnityEngine.Random.Range(50,57);
		}
		tempLocation = new Vector3(randX,2,randZ);
		obj = Instantiate(Lightning,tempLocation, Quaternion.identity) as GameObject;
		for(int i = 0; i < instantiatedSpeed.Length; ++i){
			if(instantiatedSpeed[i] == false){
				speedVec[i] = obj;
				break;	
			}
		}
		++numInstantiatedSpeed;
	}
	
}
