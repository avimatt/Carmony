using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ResetToMap : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider coll){
        //This threw an exception...
        if (!coll.GetComponentInParent<Transform>() || !coll.GetComponentInParent<Transform>().GetComponentInParent<UserInteraction>())
            return;
		bool isBottomScreen = coll.GetComponentInParent<Transform>().GetComponentInParent<UserInteraction>().isCarBottom;
		if (!isBottomScreen)
		{
			Main.S.carTop.GetComponent<UserInteraction>().startReset();
            CarmonyGUI.S.topSwapText.SetActive(true);
            CarmonyGUI.S.topSwapText.GetComponent<Text>().text = "Resetting To Track";
		}
		else
		{
			Main.S.carBottom.GetComponent<UserInteraction>().startReset();
            CarmonyGUI.S.bottomSwapText.SetActive(true);
            CarmonyGUI.S.bottomSwapText.GetComponent<Text>().text = "Resetting To Track";
        }
	
	}
}
