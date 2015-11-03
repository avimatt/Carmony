using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;
using InControl;

public class Main : MonoBehaviour
{
    static public Main S;

    public float interactTimer;

    public GameObject carTop;
    public GameObject carBottom;

    public bool carTopDone;
    public bool carBottomDone;

	public bool paused;

    public int totalLaps = 3;

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
        if (canInteract())
        {
            if (getStartPressed())
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
        int minutes = (int)(Time.time / 60);
        int seconds = (int)(Time.time % 60);
        string secondString = seconds.ToString();
        if (seconds < 10)
            secondString = "0" + seconds.ToString();
        return minutes + ":" + secondString;
    }
    // Display end game screen from the team that finished
    public void endGame(bool isTop)
    {
        if (isTop)
        {
            carTopDone = true;
            CarmonyGUI.S.topMinimap.SetActive(false);
            CarmonyGUI.S.topImageLeft.SetActive(false);
            CarmonyGUI.S.topImageRight.SetActive(false);
        }
        else
        {
            CarmonyGUI.S.bottomMinimap.SetActive(false);
            CarmonyGUI.S.bottomImageLeft.SetActive(false);
            CarmonyGUI.S.bottomImageRight.SetActive(false);
            carBottomDone = true;
        }
        CarmonyGUI.S.endGame(isTop);
    }
}