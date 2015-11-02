using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

using System.Collections;
using InControl;
using UnityStandardAssets.Vehicles.Car;

public class StartScreen : MonoBehaviour {

    static public StartScreen S;
    public GameObject text;
    int countSet = 0;
    public List<GameObject> buttonList;
    public List<bool> playersSet;
    public List<bool> buttonsSet;

    public float cooldown;

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
        if (countSet == InputManager.Devices.Count && Time.realtimeSinceStartup - cooldown > .25)
        {
            for (int i = 0; i < InputManager.Devices.Count; i++)
            {
                var player = InputManager.Devices[i];
                if (player.AnyButton)
                {
                    gameObject.SetActive(false);
                    cooldown = Time.realtimeSinceStartup;
                    InstructionScreen.S.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            for (int i = 0; i < InputManager.Devices.Count; i++)
            {
                if (playersSet[i])
                    continue;
                var player = InputManager.Devices[i];
                if ((player.Action1 && !buttonsSet[0]) || (player.Action2 && !buttonsSet[1]) || (player.Action3 && !buttonsSet[2]) || (player.Action4 && !buttonsSet[3]))
                {
                    playersSet[i] = true;
                    buttonClicked(player,i);
                }
            }
        }
	}

    void buttonClicked(InputDevice player,int playerIndex)
    {
        if (player.Action1)
        {
            buttonList[0].GetComponent<Image>().color = new Color32(50, 50, 50, 255);
            buttonsSet[0] = true;
            Main.S.carBottom.GetComponent<CarUserControl>().first = playerIndex;
        }else if (player.Action2)
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
