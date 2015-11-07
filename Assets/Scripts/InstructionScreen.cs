using UnityEngine;
using System.Collections;
using InControl;

public class InstructionScreen : MonoBehaviour {
	
	static public InstructionScreen S;
	
	public GameObject[] controlGroups = new GameObject[2];
	
	
	private bool firstGroup;
	private bool switchingGroups;
	void Awake()
	{
		S = this;
	}
	
	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
		firstGroup = true;
		switchingGroups = false;
	}
	
	// Update is called once per frame
	void Update()
	{
		if (Time.realtimeSinceStartup- StartScreen.S.cooldown  > .2)
		{
			for (int i = 0; i < InputManager.Devices.Count; i++)
			{
				var player = InputManager.Devices[i];
				// after both control groups have been seen
				if (player.AnyButton && !firstGroup && controlGroups[1].transform.localPosition[0] <= 0)
				{
					Time.timeScale = 1;
                    Main.S.practicing = true;
					gameObject.SetActive(false);
					GameObject.Find("MainGameObject").GetComponent<AudioSource>().enabled = true;
				} 
				// after first control group shown
				else if(player.AnyButton)
				{
					switchingGroups = true;
					firstGroup = false;
				}
			}
		}
		
		if (switchingGroups) {
			switchGroups ();
		}
	}
	
	void switchGroups(){
		if(controlGroups[1].transform.localPosition[0] > 0){
			Vector3 temp = controlGroups[0].transform.position;
			temp[0] -= 40;
			controlGroups[0].transform.position = temp;
			temp = controlGroups[1].transform.position;
			temp[0] -= 40;
			controlGroups[1].transform.position = temp;
		} else {
			switchingGroups = false;
		}
	}
	
}
