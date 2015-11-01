//tweak the direction of the swapArrows and put in the speed up

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class PowerupGenerator : MonoBehaviour {
	public static PowerupGenerator S;
	public Vector3[] swapGeneration;
	private Vector3 turn90;
	public bool[] instantiatedSwap, instantiatedSpeed;
	public int numPowerups;
	private int rand, numInstantiatedSwap, numInstantiatedSpeed;
	private GameObject obj;
	public GameObject SwapArrow;
	// Use this for initialization
	public GameObject[] swapVec, speedVec;
	void Start () {
		S = this;
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
		for(int i = 0; i < numPowerups; ++i){
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
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
