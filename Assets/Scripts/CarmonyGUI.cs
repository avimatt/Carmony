using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

using InControl;

public class CarmonyGUI : MonoBehaviour {

    static public CarmonyGUI S;
    public GameObject topGUI;
    public GameObject bottomGUI;
    public GameObject progressBar;
    public GameObject timeText;
    public GameObject topLetters;
	//public powerUpType topType;
    public List<GameObject> topLetterList;
    public GameObject bottomLetters;
    public List<GameObject> bottomLetterList;
	//public powerUpType bottomType;

    public GameObject topEnd;
    public GameObject bottomEnd;
    public GameObject topEndPlace;
    public GameObject topEndTime;
    public GameObject bottomEndPlace;
    public GameObject bottomEndTime;

    public GameObject restartText;

    public GameObject topMinimapDots;
    public GameObject bottomMinimapDots;

    public GameObject topSwapText;
    public GameObject bottomSwapText;

    public GameObject topImageLeft;
    public GameObject topImageRight;
    public GameObject bottomImageLeft;
    public GameObject bottomImageRight;

    public Sprite abutton;
    public Sprite bbutton;
    public Sprite xbutton;
    public Sprite ybutton;
    public Sprite upbutton;
    public Sprite downbutton;
    public Sprite leftbutton;
    public Sprite rightbutton;

	public GameObject topMinimap;
	public GameObject bottomMinimap;

    bool inLettersTop;
    bool inLettersBottom;
    int curIndexBottom = 0;
    int curIndexTop = 0;
    List<string> letterListTop;
    List<string> letterListBottom;

    public AudioClip waitClip;
    public AudioClip goClip;
    //public GameObject goBoard;
    public Material waitMat;
    public Material goMat;
    public GameObject goText;
    public GameObject startFireworks;

    public GameObject topSpeed;
    public GameObject topLap;
    public GameObject bottomSpeed;
    public GameObject bottomLap;


    public GameObject practiceText;
    public GameObject topPlate;
    public GameObject bottomPlate;

    public Image powerupImageTop;
    public Image powerupImageBottom;
    // Activation 'A' button references
    public Image topActivationButton, topActivationHighlight, topActivationSlider, bottomActivationButton, bottomActivationHighlight, bottomActivationSlider;
    public float activationTime = 2.0f;
    private bool topHasPowerup = false, bottomHasPowerup = false;
    public powerUpType topType, bottomType;
    public Sprite swapSprite;
    public Sprite speedSprite;
    public Sprite oilSprite;
	public Sprite portalSprite;

    public GameObject topPerfect;
    public GameObject bottomPerfect;

    void Awake()
    {

        S = this;
    }

	// Use this for initialization
	void Start () {
        topMinimap.SetActive(false);
        bottomMinimap.SetActive(false);
		topMinimapDots.SetActive(false);
		bottomMinimapDots.SetActive(false);

		topType = powerUpType.empty;
		bottomType = powerUpType.empty;

        bottomImageRight.SetActive(false);
        bottomImageLeft.SetActive(false);
        topImageRight.SetActive(false);
        topImageLeft.SetActive(false);
        // Activation system starts hidden, until someone gets a powerup.
        this.HideActivationButton();
    }

    // Hide the new activation system
    public void HideActivationButton()
    {
        
        this.HideTopPowerUpActivator();
        this.HideBottomPowerUpActivator();
    }

    // Display the power up activation button on the screen
    public void showActivationLetter(bool isTopScreen, powerUpType pwr_t)
    {
        if (isTopScreen)
        {
            this.topActivationButton.enabled = true;
        }
        else
        {
            this.bottomActivationButton.enabled = true;
        }
    }

    //makes coroutine callable by other classes
    public void raiseStartFlagText()
    {
        StartCoroutine("startFlagText");
    }


    //Co-routine to display GO when race starts then take display down
    //This utilizes swap text. if need to stylize, must create new text
    IEnumerator startFlagText()
    {
        goText.SetActive(true);
        gameObject.GetComponent<AudioSource>().enabled = true;
        //goText.GetComponent<Text>().color = new Color32(0, 0, 0, 255);

        goText.GetComponent<Text>().text = "3";
        gameObject.GetComponent<AudioSource>().clip = waitClip;
        gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1f);

        goText.GetComponent<Text>().text = "2";
        gameObject.GetComponent<AudioSource>().clip = waitClip;
        gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1f);

        goText.GetComponent<Text>().text = "1";
        gameObject.GetComponent<AudioSource>().clip = waitClip;
        gameObject.GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(1f);

        gameObject.GetComponent<AudioSource>().clip = goClip;
        gameObject.GetComponent<AudioSource>().Play();

        goText.GetComponent<Text>().text = "G O !";
        goText.GetComponent<Text>().color = new Color32(56, 139, 0, 255);
        //these two lines do nothing right now.
        //goBoard.GetComponent<MeshRenderer>().materials.SetValue(goMat,0);
        //goBoard.GetComponent<MeshRenderer>().material = goMat;

        Main.S.setRaceStarted();
        //StartCoroutine("startstartFireworks");
        showInitialUI();
        yield return new WaitForSeconds(1f);
		print ("here --- yay!");
        goText.SetActive(false);
        //StartCoroutine("closeStartLine");
    }

    void showInitialUI()
    {
        CarmonyGUI.S.bottomMinimap.SetActive(true);
        CarmonyGUI.S.topMinimap.SetActive(true);
        CarmonyGUI.S.topMinimapDots.SetActive(true);
        CarmonyGUI.S.bottomMinimapDots.SetActive(true);
        CarmonyGUI.S.bottomImageLeft.SetActive(true);
        CarmonyGUI.S.bottomImageRight.SetActive(true);
        CarmonyGUI.S.topImageLeft.SetActive(true);
        CarmonyGUI.S.topImageRight.SetActive(true);
        topSpeed.SetActive(true);
        topLap.SetActive(true);
        bottomSpeed.SetActive(true);
        bottomLap.SetActive(true);
        
    }

    /*IEnumerator startstartFireworks()
    {
        startFireworks.SetActive(true);
        yield return new WaitForSeconds(5);
        startFireworks.SetActive(false);

    }

    IEnumerator closeStartLine()
    {
        yield return new WaitForSeconds(30);
        goBoard.SetActive(false);
    }*/

	// Return the current index of which letter in the powerup sequence the player is at
    int getCurIndex(bool isTop)
    {
        if (isTop)
            return curIndexTop;
        else
            return curIndexBottom;
    }

	// Returns whether a button was pressed corresponding to the sequence
	// TODO Is there a reason this doesn't return a bool. It seems like the hit variable in update is used like a bool
    bool getHit(bool inLettersTop)
    {
		// Get device objects for the correct team
		CarUserControl userContorl = inLettersTop ? Main.S.carTop.GetComponent<CarUserControl> () : Main.S.carBottom.GetComponent<CarUserControl> ();
		var playerAInput = InputManager.Devices[userContorl.first];
        var playerBInput = InputManager.Devices[userContorl.second];

		// Get which letter to check for
        string letter = "";
        if (inLettersTop)
            letter = letterListTop[curIndexTop];
        else
            letter = letterListBottom[curIndexBottom];

		// Check for player input of the correct letter
        bool hit = false;
        int curIndex = getCurIndex(inLettersTop);
        
        switch (letter)
        {
            case "A":
                if (curIndex % 2 == 0)
                    hit = playerAInput.Action1.WasPressed;
                else
                    hit = playerBInput.Action1.WasPressed;
                break;
            case "B":
                if (curIndex % 2 == 0)
                    hit = playerAInput.Action2.WasPressed;
                else
                    hit = playerBInput.Action2.WasPressed;
                break;
            case "X":
                if (curIndex % 2 == 0)
                    hit = playerAInput.Action3.WasPressed;
                else
                    hit = playerBInput.Action3.WasPressed;
                break;
            case "Y":
                if (curIndex % 2 == 0)
                    hit = playerAInput.Action4.WasPressed;
                else
                    hit = playerBInput.Action4.WasPressed;
                break;
        }
        return hit;
    }

    void Update()
    {
        if (!Main.S.practicing && !Main.S.raceStarted)
            return;
        // Get references to the control objects for both teams
        CarUserControl topUserContorl = Main.S.carTop.GetComponent<CarUserControl> ();
        var topPlayerAInput = InputManager.Devices[topUserContorl.first];
        var topPlayerBInput = InputManager.Devices[topUserContorl.second];
        CarUserControl bottomUserControl = Main.S.carBottom.GetComponent<CarUserControl> ();
        var bottomPlayerAInput = InputManager.Devices[0];
        var bottomPlayerBInput = InputManager.Devices[0];

        if (bottomUserControl.first < InputManager.Devices.Count)
        {
            bottomPlayerAInput = InputManager.Devices[bottomUserControl.first];
            bottomPlayerBInput = InputManager.Devices[bottomUserControl.second];
        }

        // If a team has a powerup, check and see if they are hitting the buttons
        if (this.topHasPowerup)
        {
            // If only one player is pressing 'A', then highlight the 'A' button.
            if (topPlayerAInput.Action1.IsPressed ^ topPlayerBInput.Action1.IsPressed)
            {
                if (!this.topActivationHighlight.enabled) this.topActivationHighlight.enabled = true;
                this.topActivationSlider.fillAmount = 0f;
            }
            // If both are pressing at the same time, then start building up the slider
            else if (topPlayerAInput.Action1.IsPressed && topPlayerBInput.Action1.IsPressed)
            {
                if (!this.topActivationHighlight.enabled) this.topActivationHighlight.enabled = true;
                this.topActivationSlider.fillAmount += Time.deltaTime / this.activationTime;
                // If both have held it for long enough, activate the powerup
                if (this.topActivationSlider.fillAmount >= 1.0f)
                {
                    PowerUp.ActivatePowerUp(true, topType);
                    topType = powerUpType.empty;
                    this.topHasPowerup = false;
                    this.HideTopPowerUpActivator();
                }
            }
            else
            {
                if (this.topActivationHighlight.enabled) this.topActivationHighlight.enabled = false;
                this.topActivationSlider.fillAmount = 0f;
            }
        }
        if (this.bottomHasPowerup)
        {
            if (bottomPlayerAInput.Action1.IsPressed ^ bottomPlayerBInput.Action1.IsPressed)
            {
                if (!this.bottomActivationHighlight.enabled) this.bottomActivationHighlight.enabled = true;
                this.bottomActivationSlider.fillAmount = 0f;
            }
            else if (bottomPlayerAInput.Action1.IsPressed && bottomPlayerBInput.Action1.IsPressed)
            {
                if (!this.bottomActivationHighlight.enabled) this.bottomActivationHighlight.enabled = true;
                this.bottomActivationSlider.fillAmount += Time.deltaTime / this.activationTime;
                if (this.bottomActivationSlider.fillAmount >= 1.0f)
                {
                    PowerUp.ActivatePowerUp(false, bottomType);
                    bottomType = powerUpType.empty;
                    this.bottomHasPowerup = false;
                    this.HideBottomPowerUpActivator();
                }
            }
            else
            {
                if (this.bottomActivationHighlight.enabled) this.bottomActivationHighlight.enabled = false;
                this.bottomActivationSlider.fillAmount = 0f;
            }
        }
    }

    void HideTopPowerUpActivator()
    {
        powerupImageTop.enabled = false;
        this.topActivationButton.enabled = false;
        this.topActivationHighlight.enabled = false;
        this.topActivationSlider.fillAmount = 0f;
    }

    void HideBottomPowerUpActivator()
    {
        powerupImageBottom.enabled = false;
        this.bottomActivationButton.enabled = false;
        this.bottomActivationHighlight.enabled = false;
        this.bottomActivationSlider.fillAmount = 0f;
    }
	

    public void hideGUI()
    {
        if (Main.S.practicing)
        {
            print("here idk where you are2?");
            PracticeMap.S.practiceText.SetActive(false);
            PracticeMap.S.topPlate.SetActive(false);
            PracticeMap.S.bottomPlate.SetActive(false);
        }
        else
        {
            topGUI.SetActive(false);
            bottomGUI.SetActive(false);
            //timeText.SetActive(false);
            topLetters.SetActive(false);
            topMinimap.SetActive(false);
            bottomMinimap.SetActive(false);
            topMinimapDots.SetActive(false);
            bottomMinimapDots.SetActive(false);
            bottomImageLeft.SetActive(false);
            bottomImageRight.SetActive(false);
            topImageLeft.SetActive(false);
            topImageRight.SetActive(false);
        }
    }

    public void showGUI()
    {

        if (Main.S.practicing)
        {
            PracticeMap.S.practiceText.SetActive(true);
            PracticeMap.S.topPlate.SetActive(true);
            PracticeMap.S.bottomPlate.SetActive(true);
        }
        else
        {
            topGUI.SetActive(true);
            bottomGUI.SetActive(true);
            //timeText.SetActive(true);
            topLetters.SetActive(true);
            topMinimap.SetActive(true);
            topMinimapDots.SetActive(true);
            bottomMinimapDots.SetActive(true);
            bottomMinimap.SetActive(true);
            bottomImageLeft.SetActive(true);
            bottomImageRight.SetActive(true);
            topImageLeft.SetActive(true);
            topImageRight.SetActive(true);
        }
    }


    //return the image of the xbox button corresponding to the letter
    Sprite getSpriteForLetter(string letter)
    {
        if (letter == "A")
            return abutton;
        else if (letter == "B")
            return bbutton;
        else if (letter == "X")
            return xbutton;
        else
            return ybutton;
    }

    // Enable the power up for a particular team. Displays the a button for that team.
    public void GiveTeamPowerup(bool isTopTeam, powerUpType type)
    {
        if (isTopTeam)
        {
            this.topHasPowerup = true;
            this.topType = type;
            this.topActivationButton.enabled = true;
            powerupImageTop.enabled = true;
            powerupImageTop.sprite = getPowerupImage(type);
        }
        else
        {
            this.bottomHasPowerup = true;
            this.bottomType = type;
            this.bottomActivationButton.enabled = true;
            powerupImageBottom.enabled = true;
            powerupImageBottom.sprite = getPowerupImage(type);
        }
    }

    //returns the image corresponding to a specific powerup
    public Sprite getPowerupImage(powerUpType type)
    {
        switch (type)
        {
            case powerUpType.swap:
                return swapSprite;
            case powerUpType.speed:
                return speedSprite;
            case powerUpType.oil:
                return oilSprite;
			case powerUpType.portal:
				return portalSprite;
            default:
                return speedSprite;
        }

    }

	// Display all end game scenarios
    public void endGame(bool isTop)
    {
        if (isTop)
        {
            topEnd.SetActive(true);
            topEndTime.GetComponent<Text>().text = Main.S.getGameTime();
            if (!Main.S.carBottomDone)
                topEndPlace.GetComponent<Text>().text = "1st";
            else
                topEndPlace.GetComponent<Text>().text = "2nd";
        }
        else
        {
            bottomEnd.SetActive(true);
            bottomEndTime.GetComponent<Text>().text = Main.S.getGameTime();
            if (!Main.S.carTopDone)
                bottomEndPlace.GetComponent<Text>().text = "1st";
            else
                bottomEndPlace.GetComponent<Text>().text = "2nd";
        }

        if (Main.S.carBottomDone && Main.S.carTopDone)
        {
            restartText.SetActive(true);
        }
    }
}