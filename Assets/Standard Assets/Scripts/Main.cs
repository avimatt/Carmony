using UnityEngine;
using System.Collections;
using UnityStandardAssets.CrossPlatformInput;

public class Main : MonoBehaviour
{
    static public Main S;

    public bool paused;

    public float interactTimer;

    public GameObject carTop;
    public GameObject carBottom;

    //updates the most recent pausing interaction to now, so the player cannot do anything too soon. like spam pause
    public void updateInteractTimer()
    {
        interactTimer = Time.time;
    }

    //Is the player allowed to pause ect?
    public bool canInteract()
    {
        if (paused)
        {
            return false;
        }else if (Time.time - interactTimer < .5)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

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
                Main.S.paused = true;
                PauseScreen.S.gameObject.SetActive(true);
            }
        }
    }


    public bool getStartPressed()
    {
        //this wont work with the time shit...
        //if (CrossPlatformInputManager.GetAxis("Cancel") != 0)
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}