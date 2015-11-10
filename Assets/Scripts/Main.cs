using UnityEngine;
using System.Collections;
using InControl;
using System.Collections.Generic;
using UnityStandardAssets.Vehicles.Car;

public class Main : MonoBehaviour
{
    static public Main S;

    public float interactTimer;

    public GameObject carTop;
    public GameObject carBottom;

    public List<GameObject> fireworkList;

    public bool carTopDone;
    public bool carBottomDone;

	public bool paused;

    public int totalLaps = 3;

    public bool raceStarted;
    public bool practicing;

    public GameObject Map;

    public int carsReady = 0;

    public float startTime;
    public void setCarReady()
    {
        if (carsReady < 2)
            carsReady++;
        if (carsReady == 2 && !raceStarted)
        {
            CarmonyGUI.S.raiseStartFlagText();
        }
    }

    public bool getRaceStarted()
    {
        return raceStarted;
    }
    public void setRaceStarted()
    {
        startTime = Time.time;
        raceStarted = true;
    }


    //returns true if device is handling input for top car else it returns false
    public bool isFromTopCar(int player)
    {
        if (player == Main.S.carTop.GetComponent<CarUserControl>().first || player == Main.S.carTop.GetComponent<CarUserControl>().second)
            return true;
        return false;
    }

    //updates the most recent pausing interaction to now, so the player cannot do anything too soon. like spam pause
    public void updateInteractTimer()
    {
        interactTimer = Time.time;
    }

    //Is the player allowed to pause
    public bool canInteract()
    {
        if (paused)
        {
            return false;
        }
		else if (Time.time - interactTimer < .5)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    void Awake()
    {
        S = this;
    }

    // Use this for initialization
    void Start()
    {
        S = this;
        interactTimer = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        CarState.isCarBehind(true);

        if (canInteract())
        {
            if (getStartPressed() && (practicing || raceStarted))
            {
           		// If both teams have finished and start has been hit restart the game
                if (carTopDone && carBottomDone)
                {
                    Application.LoadLevel("NoahDevScene");
                }
                Main.S.paused = true;
                PauseScreen.S.gameObject.SetActive(true);
            }
        }
    }

	// Return whether any player has pressed start
    public bool getStartPressed()
    {
        //this wont work with the time shit...
        //if (CrossPlatformInputManager.GetAxis("Cancel") != 0)

        for (int i = 0; i < InputManager.Devices.Count; i++)
        {
            var player = InputManager.Devices[i];
            if (player.MenuWasPressed)
            {
                return true;
            }
        }
        return false;

    }
    public string getGameTime()
    {
        int minutes = (int)((Time.time-startTime) / 60);
        int seconds = (int)((Time.time-startTime) % 60);
        string secondString = seconds.ToString();
        if (seconds < 10)
            secondString = "0" + seconds.ToString();
        return minutes + ":" + secondString;
    }
    // Display end game screen from the team that finished
    public void endGame(bool isTop)
    {
        foreach(GameObject go in fireworkList)
        {
            go.SetActive(true);
        }
        if (isTop)
        {
            carTopDone = true;
            CarmonyGUI.S.topMinimap.SetActive(false);
			CarmonyGUI.S.topMinimapDots.SetActive(false);
            CarmonyGUI.S.topImageLeft.SetActive(false);
            CarmonyGUI.S.topImageRight.SetActive(false);
            carTop.GetComponent<CarState>().totalTime = getGameTime();
        }
        else
        {
            CarmonyGUI.S.bottomMinimap.SetActive(false);
			CarmonyGUI.S.bottomMinimapDots.SetActive(false);
            CarmonyGUI.S.bottomImageLeft.SetActive(false);
            CarmonyGUI.S.bottomImageRight.SetActive(false);
            carBottom.GetComponent<CarState>().totalTime = getGameTime();
            carBottomDone = true;
        }
        Logger.S.printSummary(isTop);
        CarmonyGUI.S.endGame(isTop);
    }
}