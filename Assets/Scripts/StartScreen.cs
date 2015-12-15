using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using System.Collections;
using InControl;

public class StartScreen : MonoBehaviour {

    static public StartScreen S;

    public GameObject text;
    
    public List<GameObject> buttonList; // Player 1,2,3,4 buttons on the screen
    public List<bool> playersSet; // Whether a given player has chosen a team
    public List<bool> buttonsSet; // Whether Player 1,2,3 or 4 (as determined from the button list) has been taken

    public GameObject topCar;
    public GameObject bottomCar;
    public Text topNameText;
    public Text bottomNameText;
    public float cooldown;

	int countSet = 0; // # of players who have chosen a team 

    void Awake()
    {
        S = this;
    }

	// Use this for initialization
	void Start () {
        playersSet.Add(false);
        playersSet.Add(false);
        playersSet.Add(false);
        playersSet.Add(false);

        buttonsSet.Add(false);
        buttonsSet.Add(false);
        buttonsSet.Add(false);
        buttonsSet.Add(false);
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update () {

        topNameText.text = Main.S.carTop.GetComponent<CarState>().name;
        bottomNameText.text = Main.S.carBottom.GetComponent<CarState>().name;

        // If all connected devices have chosen a team
        Vector3 topAngle = topCar.transform.rotation.eulerAngles;
        Vector3 bottomAngle = bottomCar.transform.rotation.eulerAngles;
        topAngle.y += 1;
        bottomAngle.y += 1;
        topCar.transform.rotation = Quaternion.Euler(topAngle);
        bottomCar.transform.rotation = Quaternion.Euler(bottomAngle);
        if (countSet == InputManager.Devices.Count && Time.realtimeSinceStartup - cooldown > .25)
        {
			// Check each device for input
            for (int i = 0; i < InputManager.Devices.Count; i++)
            {
                var player = InputManager.Devices[i];
				// if input then show the instruction screen
                if (player.AnyButton.WasPressed)
                {
                    gameObject.SetActive(false);
                    cooldown = Time.realtimeSinceStartup;
                    InstructionScreen.S.gameObject.SetActive(true);
                }
            }
        }
		// Wait for input (choose player) from connected devices 
        else
        {
			// loop through each connected device
            for (int i = 0; i < InputManager.Devices.Count; i++)
            {
				// If player already has a team ignore input
                if (playersSet[i])
                    continue;
                var player = InputManager.Devices[i];
				// If player clicked button and spot not taken
                if ((player.Action2.WasPressed && !buttonsSet[0]) || (player.Action1.WasPressed && !buttonsSet[1]) || (player.Action4.WasPressed && !buttonsSet[2]) || (player.Action3.WasPressed && !buttonsSet[3]))
                {
                    playersSet[i] = true;
                    buttonClicked(player,i);
                }
            }
        }
	}

    IEnumerator vibrateControllerStop(InputDevice player)
    {
        float pauseEndTime = Time.realtimeSinceStartup + .2f;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        player.Vibrate(0, 0);
    }


    // Map controllers to players
    void buttonClicked(InputDevice player,int playerIndex)
    {
        player.Vibrate(.2f, .2f);
        StartCoroutine("vibrateControllerStop", player);
        if (player.Action2.WasPressed)
        {
            buttonList[0].GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            buttonList[0].GetComponentInChildren<Text>().enabled = false;
            buttonsSet[0] = true;
            Main.S.carTop.GetComponent<ArcadeVehicle>().first = playerIndex;
        }
        else if (player.Action1.WasPressed)
        {
            buttonList[1].GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            buttonList[1].GetComponentInChildren<Text>().enabled = false;
            buttonsSet[1] = true;
            Main.S.carTop.GetComponent<ArcadeVehicle>().second = playerIndex;
        }
        else if (player.Action4.WasPressed)
        {
            buttonList[2].GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            buttonList[2].GetComponentInChildren<Text>().enabled = false;
            buttonsSet[2] = true;
            Main.S.carBottom.GetComponent<ArcadeVehicle>().first = playerIndex;
        }
        else if (player.Action3.WasPressed)
        {
            buttonList[3].GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            buttonList[3].GetComponentInChildren<Text>().enabled = false;
            buttonsSet[3] = true;
            Main.S.carBottom.GetComponent<ArcadeVehicle>().second = playerIndex;
        }

        countSet++;
		// If all connected devices have chosen a team prompt for start of game
        if (countSet == InputManager.Devices.Count)
        {
            text.GetComponent<Text>().text = "Press Any Button To Continue";
            for (int i = 0; i < buttonsSet.Count; i++)
            {
                if (buttonsSet[i] == false)
                {
                    if (i == 0 && buttonsSet[i + 1] == true)
                    {
                        Main.S.carTop.GetComponent<ArcadeVehicle>().first = Main.S.carTop.GetComponent<ArcadeVehicle>().second;
                    }
                    else if (i == 1 && buttonsSet[i - 1] == true)
                    {
                        Main.S.carTop.GetComponent<ArcadeVehicle>().second = Main.S.carTop.GetComponent<ArcadeVehicle>().first;
                    }
                    else if (i == 2 && buttonsSet[i + 1] == true)
                    {
                        Main.S.carBottom.GetComponent<ArcadeVehicle>().first = Main.S.carBottom.GetComponent<ArcadeVehicle>().second;
                    }
                    else if (i == 3 && buttonsSet[i - 1] == true)
                    {
                        Main.S.carBottom.GetComponent<ArcadeVehicle>().second = Main.S.carBottom.GetComponent<ArcadeVehicle>().first;
                    }
                }
            }
            cooldown = Time.realtimeSinceStartup;
        }
    }
}
