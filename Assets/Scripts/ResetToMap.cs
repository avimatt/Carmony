using UnityEngine;
using System.Collections;

public class ResetToMap : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider coll){
		bool isBottomScreen = coll.GetComponentInParent<Transform>().GetComponentInParent<UserInteraction>().isCarBottom;
		if (!isBottomScreen)
		{
			Main.S.carTop.GetComponent<UserInteraction>().startReset();
		}
		else
		{
			Main.S.carBottom.GetComponent<UserInteraction>().startReset();
		}
	
	}
}
