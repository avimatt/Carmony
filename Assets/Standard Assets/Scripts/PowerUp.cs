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

    List<string> xboxLetters = new List<string>();
    
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

    void OnTriggerEnter(Collider coll)
    {
//        PowerupGenerator.S.
        Destroy(gameObject);
        if (type == powerUpType.speed)
            coll.GetComponentInParent<Transform>().GetComponentInParent<UserInteraction>().startBoost();
        else if (type == powerUpType.letters)
        {
            bool isTopScreen = coll.GetComponentInParent<Transform>().GetComponentInParent<UserInteraction>().isCarTop;
            List<string> letterList = getNewLetterList();
            CarmonyGUI.S.setLetters(isTopScreen, letterList, type);
        }else if (type == powerUpType.swap)
        {
            coll.GetComponentInParent<Transform>().GetComponentInParent<CarUserControl>().playerSwap();
        }

        //coll.gameObject.GetComponent<UserInteraction>().startBoost();
    }

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
