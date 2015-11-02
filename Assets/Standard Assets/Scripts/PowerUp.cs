using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using InControl;
using UnityStandardAssets.Vehicles.Car;

public enum powerUpType
{
	speed,
	letters,
	swap,
	empty
}

public class PowerUp : MonoBehaviour
{

    List<string> xboxLetters = new List<string>(); // List of XBox controller letters
    
    public powerUpType type;
    // Use this for initialization
    void Start()
    {
        xboxLetters.Add("X");
        xboxLetters.Add("Y");
        xboxLetters.Add("B");
        xboxLetters.Add("A");
    }

    // Update is called once per frame
    void Update()
    {

    }

	// When Player has collided with the power up
    void OnTriggerEnter(Collider coll)
    {

		print("triggerd");
		// Remove it from the scene
        Destroy(gameObject); 
        
		// Determine which car hit it
        bool isTopScreen = coll.GetComponentInParent<Transform>().GetComponentInParent<UserInteraction>().isCarTop;
		// Generate random activation sequence
        List<string> letterList = getNewLetterList();
		// Show the player the sequence
        CarmonyGUI.S.setLetters(isTopScreen, letterList, type);
        
    }

	// Generate random list of 4 letters
    List<string> getNewLetterList()
    {
        List<string> letterList = new List<string>();
        int numLetters = 4;
        for (int i = 0; i < numLetters; i++)
        {
            int randInt = Random.Range(0, 4);
            letterList.Add(xboxLetters[randInt]);
        }
        return letterList;
    }

	// Carry out the power ups action (To be called after the player has inputed the full sequence)
	public static void ActivatePowerUp(bool topPlayer, powerUpType type){

		if (type == powerUpType.speed) {
			if(topPlayer){
				Main.S.carTop.GetComponent<UserInteraction>().startBoost();
			} else {
				Main.S.carBottom.GetComponent<UserInteraction>().startBoost();
			}
			PowerupGenerator.S.numInstantiatedSpeed--;
		} else if(type == powerUpType.swap) {
			if(topPlayer){
				Main.S.carBottom.GetComponent<CarUserControl>().playerSwap();
			} else {
				Main.S.carTop.GetComponent<CarUserControl>().playerSwap();
			}
			PowerupGenerator.S.numInstantiatedSwap--;
		}

	}

}
