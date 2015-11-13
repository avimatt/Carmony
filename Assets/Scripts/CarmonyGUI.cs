using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

using InControl;

public class CarmonyGUI : MonoBehaviour {

    private AudioSource m_audiosource;
    private Text m_goText;
    private CarUserControl m_carTopUserControl;
    private CarUserControl m_carBottomUserControl;
    static public CarmonyGUI S;
    public GameObject topGUI;
    public GameObject bottomGUI;
    public GameObject topLetters;

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

    public Material waitMat;
    public Material goMat;
    public GameObject goText;

    public GameObject topSpeed;
    public GameObject topLap;
    public GameObject bottomSpeed;
    public GameObject bottomLap;

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


    public GameObject movingToPractice;
    void Awake()
    {

        S = this;
        m_audiosource = gameObject.GetComponent<AudioSource>();
        m_carTopUserControl =  Main.S.carTop.GetComponent<CarUserControl>();
        m_carBottomUserControl =  Main.S.carBottom.GetComponent<CarUserControl>();
        m_goText = goText.GetComponent<Text>();
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
        m_audiosource.enabled = true;
        //goText.GetComponent<Text>().color = new Color32(0, 0, 0, 255);

        m_goText.text = "3";
        m_audiosource.clip = waitClip;
        m_audiosource.Play();
        yield return new WaitForSeconds(1f);

        m_goText.text = "2";
        m_audiosource.clip = waitClip;
        m_audiosource.Play();
        yield return new WaitForSeconds(1f);

        m_goText.text = "1";
        m_audiosource.clip = waitClip;
        m_audiosource.Play();
        yield return new WaitForSeconds(1f);

        m_audiosource.clip = goClip;
        m_audiosource.Play();

        m_goText.text = "G O !";
        m_goText.color = new Color32(56, 139, 0, 255);

        Main.S.setRaceStarted();

        showInitialUI();
        yield return new WaitForSeconds(1f);
        goText.SetActive(false);
    }

    void showInitialUI()
    {
        bottomMinimap.SetActive(true);
        topMinimap.SetActive(true);
        topMinimapDots.SetActive(true);
        topMinimap.GetComponent<Image>().sprite = Main.S.Map.GetComponent<Map>().miniMapImage;
        bottomMinimap.GetComponent<Image>().sprite = Main.S.Map.GetComponent<Map>().miniMapImage;
        bottomMinimapDots.SetActive(true);
        bottomImageLeft.SetActive(true);
        bottomImageRight.SetActive(true);
        topImageLeft.SetActive(true);
        topImageRight.SetActive(true);
        topSpeed.SetActive(true);
        topLap.SetActive(true);
        bottomSpeed.SetActive(true);
        bottomLap.SetActive(true);
        
    }


    void Update()
    {
        if (!Main.S.practicing && !Main.S.raceStarted)
            return;
        // Get references to the control objects for both teams

        var topPlayerAInput = InputManager.Devices[m_carTopUserControl.first];
        var topPlayerBInput = InputManager.Devices[m_carTopUserControl.second];

        var bottomPlayerAInput = InputManager.Devices[0];
        var bottomPlayerBInput = InputManager.Devices[0];

        if (m_carBottomUserControl.first < InputManager.Devices.Count)
        {
            bottomPlayerAInput = InputManager.Devices[m_carBottomUserControl.first];
            bottomPlayerBInput = InputManager.Devices[m_carBottomUserControl.second];
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

    public void HideTopPowerUpActivator()
    {
        powerupImageTop.enabled = false;
        this.topActivationButton.enabled = false;
        this.topActivationHighlight.enabled = false;
        this.topActivationSlider.fillAmount = 0f;
    }

    public void HideBottomPowerUpActivator()
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
            PracticeMap.S.practiceText.SetActive(false);
            PracticeMap.S.practiceText2.SetActive(false);
            PracticeMap.S.topPlate.SetActive(false);
            PracticeMap.S.bottomPlate.SetActive(false);
        }
        else
        {
            topGUI.SetActive(false);
            bottomGUI.SetActive(false);
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
            PracticeMap.S.practiceText2.SetActive(true);
            PracticeMap.S.topPlate.SetActive(true);
            PracticeMap.S.bottomPlate.SetActive(true);
        }
        else
        {
            topGUI.SetActive(true);
            bottomGUI.SetActive(true);
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