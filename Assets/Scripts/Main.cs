using UnityEngine;
using System.Collections;
using InControl;
using System.Collections.Generic;
using System;

public class Main : MonoBehaviour
{
    static public Main S;


	[Header("Set in Inspector")]
    public GameObject carTop;
    public GameObject carBottom;
	
	public GameObject Map;
	public List<GameObject> MapList;

	public bool normalControls;

	[Header("Calculated Dynamically")]
	public int carsReady = 0;
	public float interactTimer;
	public float startTime;

	public bool practicing;
	public bool raceStarted;
	public bool carTopDone;
	public bool carBottomDone;
	public bool paused;

	public bool topInCollisionZone;
	public bool bottomInCollisionZone;


	void Awake()
	{
		S = this;
	}
	
	// Use this for initialization
	void Start()
	{
		S = this;
		interactTimer = Time.time;
		topInCollisionZone = false;
		bottomInCollisionZone = false;
		Map = MapList[0];
	}
	
	// Update is called once per frame
	void Update()
	{
		CarState.isCarBehind(true);
		
		if (canInteract() && !HighScores.S.isActiveAndEnabled)
		{
			if (getStartPressed() && (practicing || raceStarted))
			{
				// If both teams have finished and start has been hit restart the game
				if (carTopDone && carBottomDone)
				{
					if (HighScores.S.recordBeaten)
					{
						print("turning on highscores");
						HighScores.S.gameObject.SetActive(true);
					}
					else
					{
						Application.LoadLevel("NoahDevScene");
					}
				}
				Main.S.paused = true;
				PauseScreen.S.gameObject.SetActive(true);
			}
		}
	}
	
    public void setCarReady()
    {
        if (carsReady < 2)
            carsReady++;
        if (carsReady == 2 && !raceStarted)
        {
            CarmonyGUI.S.movingToPractice.SetActive(false);
            CarmonyGUI.S.raiseCountdown();
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
        if (player == Main.S.carTop.GetComponent<ArcadeVehicle>().first || player == Main.S.carTop.GetComponent<ArcadeVehicle>().second)
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

    public string getTimeDifference(int minutesA,int minutesB, int secondsA, int secondsB)
    {
        int totalA = 60 * minutesA + secondsA;
        int totalB = 60 * minutesB + secondsB;

        int totalDiff = totalA - totalB;
        int newMin = totalDiff / 60;
        int newSec = totalDiff % 60;

        string final = "+" + newMin;
        if (newSec < 10)
        {
            final += ":0";
        }
        else
        {
            final += ":";
        }
        final += newSec;
        print("final");
        return final;
    }

    // Display end game screen from the team that finished
    public void endGame(bool isTop)
    {
        foreach(GameObject go in Map.GetComponent<Map>().fireworkList)
        {
            go.SetActive(true);
        }
        scoreRow newRow = new scoreRow();
        if (isTop)
        {
            newRow.setName(carTop.GetComponent<CarState>().name);
            carTopDone = true;
            CarmonyGUI.S.topMinimap.SetActive(false);
			CarmonyGUI.S.topMinimapDots.SetActive(false);
            CarmonyGUI.S.topImageLeft.SetActive(false);
            CarmonyGUI.S.topImageRight.SetActive(false);
            carTop.GetComponent<CarState>().totalTime = getGameTime();
            CarmonyGUI.S.HideTopPowerUpActivator();
        }
        else
        {
            newRow.setName(carBottom.GetComponent<CarState>().name);
            CarmonyGUI.S.bottomMinimap.SetActive(false);
			CarmonyGUI.S.bottomMinimapDots.SetActive(false);
            CarmonyGUI.S.bottomImageLeft.SetActive(false);
            CarmonyGUI.S.bottomImageRight.SetActive(false);
            carBottom.GetComponent<CarState>().totalTime = getGameTime();
            carBottomDone = true;
            CarmonyGUI.S.HideBottomPowerUpActivator();
        }
        Logger.S.printSummary(isTop);
        CarmonyGUI.S.endGame(isTop);


        int minutes = Int32.Parse(getGameTime().Substring(0, 1));
        int seconds = Int32.Parse(getGameTime().Substring(2, 2));
        newRow.setTime(minutes, seconds);
        HighScores.S.updateList(newRow);
    }
}