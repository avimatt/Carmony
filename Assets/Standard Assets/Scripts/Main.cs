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

	// Is the player allowed to move
	// TODO What is this meant for?
    public bool canMove()
    {
        if (paused)
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

	// Display end game screen from the team that finished
    public void endGame(bool isTop)
    {
        if (isTop)
            carTopDone = true;
        else
            carBottomDone = true;
        CarmonyGUI.S.endGame(isTop);
    }
}