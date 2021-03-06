using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

using InControl;
using System;

public class CarmonyGUI : MonoBehaviour {

    private AudioSource m_audiosource;
    private Text m_goText;
    //private CarUserControl m_carTopUserControl;
    //private CarUserControl m_carBottomUserControl;

	private ArcadeVehicle m_carTopArcadeVehicle;
	private ArcadeVehicle m_carBottomArcadeVehicle;

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

	public bool swappingTop;
	public bool swappingBottom;

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
    public Color activatorGreen;
    public Color activatorRed;
    public Image topActivationButton, topActivationSlider, bottomActivationButton, bottomActivationSlider;
    public Image topActivationButton2, topActivationSlider2, bottomActivationButton2, bottomActivationSlider2;

    public float activationTime = 2.0f;
    private bool topHasPowerup = false, bottomHasPowerup = false;
    public powerUpType topType, bottomType;
    public Sprite swapSprite;
    public Sprite speedSprite;
    public Sprite oilSprite;
	public Sprite portalSprite;
    public Sprite rocketSprite;

    public GameObject topPerfect;
    public GameObject bottomPerfect;

    public Image fader;

    public Text negativeTimeTop;
    public Text negativeTimeBottom;

    public GameObject topPracticeHelper;
    public GameObject bottomPracticeHelper;

    public Image topSpeedBox;
    public Image topSpeedSlider;

    public Image bottomSpeedBox;
    public Image bottomSpeedSlider;
    public void fadeOut()
    {
        Color newColor = fader.color;
        newColor.a += .005f;
        fader.color = newColor;
    }
    
    public void fadeIn()
    {
        Color newColor = fader.color;
        newColor.a -= .01f;
        fader.color = newColor;
    }

    public GameObject movingToPractice;
    void Awake()
    {

        S = this;
        m_audiosource = gameObject.GetComponent<AudioSource>();
		m_carTopArcadeVehicle =  Main.S.carTop.GetComponent<ArcadeVehicle>();
		m_carBottomArcadeVehicle =  Main.S.carBottom.GetComponent<ArcadeVehicle>();
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

        topSpeedBox.enabled = false;
        topSpeedSlider.enabled = false;
        bottomSpeedBox.enabled = false;
        bottomSpeedSlider.enabled = false;

        activatorGreen = new Color32(57, 204, 11,255);
        activatorRed = new Color32(255, 139, 139,255);
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
            this.topActivationButton2.enabled = true;

        }
        else
        {
            this.bottomActivationButton.enabled = true;
            this.bottomActivationButton2.enabled = true;

        }
    }

    //makes coroutine callable by other classes
    public void raiseCountdown()
    {
        StartCoroutine("startCountdown");
    }


    //Co-routine to display GO when race starts then take display down
    IEnumerator startCountdown()
    {
        topHasPowerup = false;
        bottomHasPowerup = false;
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
        topSpeedBox.enabled = true;
        topSpeedSlider.enabled = true;
        bottomSpeedBox.enabled = true;
        bottomSpeedSlider.enabled = true;
    }


    void Update()
    {
        if (!Main.S.practicing && !Main.S.raceStarted)
            return;
        // Get references to the control objects for both teams

		swapControlImages ();

		var topPlayerAInput = InputManager.Devices[m_carTopArcadeVehicle.first];
		var topPlayerBInput = InputManager.Devices[m_carTopArcadeVehicle.second];

        var bottomPlayerAInput = InputManager.Devices[0];
        var bottomPlayerBInput = InputManager.Devices[0];

		if (m_carBottomArcadeVehicle.first < InputManager.Devices.Count)
        {
			bottomPlayerAInput = InputManager.Devices[m_carBottomArcadeVehicle.first];
			bottomPlayerBInput = InputManager.Devices[m_carBottomArcadeVehicle.second];
        }

        if (topSpeedSlider.enabled && bottomSpeedSlider.enabled)
        {
            topSpeedSlider.fillAmount = Main.S.carTop.GetComponent<ArcadeVehicle>().getSpeed() / 100;
            bottomSpeedSlider.fillAmount = Main.S.carBottom.GetComponent<ArcadeVehicle>().getSpeed() / 100;

        }

        if (this.topHasPowerup && !Main.S.carTopDone)
        {
            // If a player is pressing the A button, turn the indicator green.
            this.topActivationSlider.color = topPlayerAInput.Action1.IsPressed ? this.activatorGreen : this.activatorRed;
            this.topActivationSlider2.color = topPlayerBInput.Action1.IsPressed ? this.activatorGreen : this.activatorRed;
            // If both pressing it, activate the powerup
            if (topPlayerAInput.Action1.IsPressed && topPlayerBInput.Action1.IsPressed && !PauseScreen.S.isActiveAndEnabled)
            {
                PowerUp.ActivatePowerUp(true, topType);
                topType = powerUpType.empty;
                this.topHasPowerup = false;
                this.HideTopPowerUpActivator();
            }
        }
        if (this.bottomHasPowerup && !Main.S.carBottomDone && !PauseScreen.S.isActiveAndEnabled)
        {
            // If a player is pressing the A button, turn the indicator green.
            this.bottomActivationSlider.color = bottomPlayerAInput.Action1.IsPressed ? this.activatorGreen : this.activatorRed;
            this.bottomActivationSlider2.color = bottomPlayerBInput.Action1.IsPressed ? this.activatorGreen : this.activatorRed;
            // If both pressing it, activate the powerup
            if (bottomPlayerAInput.Action1.IsPressed && bottomPlayerBInput.Action1.IsPressed)
            {
                PowerUp.ActivatePowerUp(false, bottomType);
                bottomType = powerUpType.empty;
                this.bottomHasPowerup = false;
                this.HideBottomPowerUpActivator();
            }
        }
    }


	bool goingUp = true;
	public void swapControlImages(){
		GameObject leftImage;
		GameObject rightImage;
		if (swappingTop) {
			leftImage = topImageLeft;
			rightImage = topImageRight;
			Vector3 leftPos = leftImage.transform.localPosition;
			Vector3 rightPos = rightImage.transform.localPosition;

			if(leftPos.x < 356.5f){
				leftPos.x += 11.7f;
				rightPos.x -= 11.7f;
				leftImage.transform.localPosition = leftPos;
				rightImage.transform.localPosition = rightPos;
			} else {
				leftPos.x = 356.5f;
				rightPos.x = -340.5f;
				leftImage.transform.localPosition = leftPos;
				rightImage.transform.localPosition = rightPos;
			}

			if (leftPos.y < 155 && goingUp) {
				leftPos.y += 5f;
				rightPos.y += 5f;
				leftImage.transform.localPosition = leftPos;
				rightImage.transform.localPosition = rightPos;
				if(leftPos.y == 155) goingUp = false;
			} else if (leftPos.y > 0 && !goingUp) {
				goingUp = false;
				leftPos.y -= 5f;
				rightPos.y -= 5f;
				leftImage.transform.localPosition = leftPos;
				rightImage.transform.localPosition = rightPos;
			} else if (leftPos.y <= 0 && !goingUp) {
				leftPos.y = 0f;
				rightPos.y = 0f;
				leftImage.transform.localPosition = leftPos;
				rightImage.transform.localPosition = rightPos;
				goingUp = true;
				swappingTop = false;

				topImageLeft = rightImage;
				topImageRight = leftImage;
			}

		} else if (swappingBottom) {
			leftImage = bottomImageLeft;
			rightImage = bottomImageRight;
			Vector3 leftPos = leftImage.transform.localPosition;
			Vector3 rightPos = rightImage.transform.localPosition;
			
			if(leftPos.x < 356.5){
				leftPos.x += 11.7f;
				rightPos.x -= 11.7f;
				leftImage.transform.localPosition = leftPos;
				rightImage.transform.localPosition = rightPos;
			} else {
				leftPos.x = 356.5f;
				rightPos.x = -340.5f;
				leftImage.transform.localPosition = leftPos;
				rightImage.transform.localPosition = rightPos;
			}
			
			if (leftPos.y <= -203 && goingUp) {
				leftPos.y += 5f;
				rightPos.y += 5f;
				leftImage.transform.localPosition = leftPos;
				rightImage.transform.localPosition = rightPos;
				if(leftPos.y == -203) goingUp = false;
			} else if (leftPos.y > -353 && !goingUp) {
				goingUp = false;
				leftPos.y -= 5f;
				rightPos.y -= 5f;
				leftImage.transform.localPosition = leftPos;
				rightImage.transform.localPosition = rightPos;
			} else if (leftPos.y <= -353 && !goingUp) {
				leftPos.y = -353f;
				rightPos.y = -353f;
				leftImage.transform.localPosition = leftPos;
				rightImage.transform.localPosition = rightPos;
				goingUp = true;
				swappingBottom = false;
				
				bottomImageLeft = rightImage;
				bottomImageRight = leftImage;
			}
		} else {
			return;
		}
	

	}

    public void HideTopPowerUpActivator()
    {
        powerupImageTop.enabled = false;
        this.topActivationButton.enabled = false;
        this.topActivationSlider.enabled = false;

        this.topActivationButton2.enabled = false;
        this.topActivationSlider2.enabled = false;
    }

    public void HideBottomPowerUpActivator()
    {
        powerupImageBottom.enabled = false;
        this.bottomActivationButton.enabled = false;
        //this.bottomActivationHighlight.enabled = false;
        this.bottomActivationSlider.enabled = false;
        this.bottomActivationButton2.enabled = false;
        //this.bottomActivationHighlight2.enabled = false;
        this.bottomActivationSlider2.enabled = false;
    }
	

    public void hideGUI()
    {
        if (Main.S.practicing)
        {
            PracticeMap.S.practiceText.SetActive(false);
            PracticeMap.S.practiceText2.SetActive(false);
            PracticeMap.S.topPlate.SetActive(false);
            PracticeMap.S.bottomPlate.SetActive(false);
            topPracticeHelper.SetActive(false);
            bottomPracticeHelper.SetActive(false);
            topGUI.SetActive(false);
            bottomGUI.SetActive(false);
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

            topSpeedBox.enabled = false;
            topSpeedSlider.enabled = false;
            bottomSpeedBox.enabled = false;
            bottomSpeedSlider.enabled = false;
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
            topGUI.SetActive(true);
            bottomGUI.SetActive(true);
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

            topSpeedBox.enabled = true;
            topSpeedSlider.enabled = true;
            bottomSpeedBox.enabled = true;
            bottomSpeedSlider.enabled = true;
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
            this.topActivationButton2.enabled = true;
            this.topActivationSlider.enabled = true;
            this.topActivationSlider2.enabled = true;

            powerupImageTop.enabled = true;
            powerupImageTop.sprite = getPowerupImage(type);
        }
        else
        {
            this.bottomHasPowerup = true;
            this.bottomType = type;
            this.bottomActivationButton.enabled = true;
            this.bottomActivationButton2.enabled = true;
            this.bottomActivationSlider.enabled = true;
            this.bottomActivationSlider2.enabled = true;
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
            case powerUpType.rocket:
                return rocketSprite;
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
            {
                topEndPlace.GetComponent<Text>().text = "2nd";
                string TopTime = Main.S.getGameTime();
                string BottomTime = bottomEndTime.GetComponent<Text>().text;
                int TopMinutes = Int32.Parse(TopTime.Substring(0, 1));
                int BottomMinutes = Int32.Parse(BottomTime.Substring(0, 1));
                int TopSeconds = Int32.Parse(TopTime.Substring(2, 2));
                int BottomSeconds = Int32.Parse(BottomTime.Substring(2, 2));
                negativeTimeTop.text = Main.S.getTimeDifference(TopMinutes, BottomMinutes, TopSeconds, BottomSeconds);
            }
        }
        else
        {
            bottomEnd.SetActive(true);
            bottomEndTime.GetComponent<Text>().text = Main.S.getGameTime();
            if (!Main.S.carTopDone)
                bottomEndPlace.GetComponent<Text>().text = "1st";
            else
            {
                bottomEndPlace.GetComponent<Text>().text = "2nd";
                string BottomTime = Main.S.getGameTime();
                string TopTime = topEndTime.GetComponent<Text>().text;
                int BottomMinutes = Int32.Parse(BottomTime.Substring(0, 1));
                int TopMinutes = Int32.Parse(TopTime.Substring(0, 1));
                int BottomSeconds = Int32.Parse(BottomTime.Substring(2, 2));
                int TopSeconds = Int32.Parse(TopTime.Substring(2, 2));
                negativeTimeBottom.text = Main.S.getTimeDifference(BottomMinutes, TopMinutes, BottomSeconds, TopSeconds);
            }
        }

        if (Main.S.carBottomDone && Main.S.carTopDone)
        {
            restartText.SetActive(true);
        }
    }
}