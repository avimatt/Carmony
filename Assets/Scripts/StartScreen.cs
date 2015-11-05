﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using System.Collections;
using InControl;
using UnityStandardAssets.Vehicles.Car;

public class StartScreen : MonoBehaviour {

    static public StartScreen S;

    public GameObject text;
    
    public List<GameObject> buttonList; // Player 1,2,3,4 buttons on the screen
    public List<bool> playersSet; // Whether a given player has chosen a team
    public List<bool> buttonsSet; // Whether Player 1,2,3 or 4 (as determined from the button list) has been taken
    public float cooldown;

	int countSet = 0; // # of players who have chosen a team 

    void Awake()
    {
        S = this;
    }

	// Use this for initialization
	void Start () {
        Time.timeScale = 0;

        playersSet.Add(false);
        playersSet.Add(false);
        playersSet.Add(false);
        playersSet.Add(false);

        buttonsSet.Add(false);
        buttonsSet.Add(false);
        buttonsSet.Add(false);
        buttonsSet.Add(false);
    }

    // Update is called once per frame
    void Update () {
		// If all connected devices have chosen a team
        if (countSet == InputManager.Devices.Count && Time.realtimeSinceStartup - cooldown > .25)
        {
			// Check each device for input
            for (int i = 0; i < InputManager.Devices.Count; i++)
            {
                var player = InputManager.Devices[i];
				// if input then show the instruction screen
                if (player.AnyButton)
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
                if ((player.Action1 && !buttonsSet[0]) || (player.Action2 && !buttonsSet[1]) || (player.Action3 && !buttonsSet[2]) || (player.Action4 && !buttonsSet[3]))
                {
                    playersSet[i] = true;
                    buttonClicked(player,i);
                }
            }
        }
	}

    IEnumerator vibrateControllerStop(InputDevice player)
    {
        print("hello1");
        float pauseEndTime = Time.realtimeSinceStartup + .2f;
        while (Time.realtimeSinceStartup < pauseEndTime)
        {
            yield return 0;
        }
        player.Vibrate(0, 0);
        print("hello2");
    }


    // Map controllers to players
    void buttonClicked(InputDevice player,int playerIndex)
    {
        player.Vibrate(.5f, .5f);
        StartCoroutine("vibrateControllerStop", player);
        if (player.Action1)
        {
            buttonList[0].GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            buttonsSet[0] = true;
            Main.S.carBottom.GetComponent<CarUserControl>().first = playerIndex;
        }
        else if (player.Action2)
        {
            buttonList[1].GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            buttonsSet[1] = true;
            Main.S.carBottom.GetComponent<CarUserControl>().second = playerIndex;
        }
        else if (player.Action3)
        {
            buttonList[2].GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            buttonsSet[2] = true;
            Main.S.carTop.GetComponent<CarUserControl>().first = playerIndex;
        }
        else if (player.Action4)
        {
            buttonList[3].GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            buttonsSet[3] = true;
            Main.S.carTop.GetComponent<CarUserControl>().second = playerIndex;
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
                        Main.S.carBottom.GetComponent<CarUserControl>().first = Main.S.carBottom.GetComponent<CarUserControl>().second;
                    }
                    else if (i == 1 && buttonsSet[i - 1] == true)
                    {
                        Main.S.carBottom.GetComponent<CarUserControl>().second = Main.S.carBottom.GetComponent<CarUserControl>().first;
                    }
                    else if (i == 2 && buttonsSet[i + 1] == true)
                    {
                        Main.S.carTop.GetComponent<CarUserControl>().first = Main.S.carTop.GetComponent<CarUserControl>().second;
                    }
                    else if (i == 3 && buttonsSet[i - 1] == true)
                    {
                        Main.S.carTop.GetComponent<CarUserControl>().second = Main.S.carTop.GetComponent<CarUserControl>().first;
                    }
                }
            }
            cooldown = Time.realtimeSinceStartup;
        }
    }
}
