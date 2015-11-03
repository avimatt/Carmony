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
	public powerUpType topType;
    public List<GameObject> topLetterList;
    public GameObject bottomLetters;
    public List<GameObject> bottomLetterList;
	public powerUpType bottomType;

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

        foreach (GameObject go in bottomLetterList)
            go.SetActive(false);
        foreach (GameObject go in topLetterList)
            go.SetActive(false);
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
        print("here");
        topSwapText.SetActive(true);
        bottomSwapText.SetActive(true);
        topSwapText.GetComponent<Text>().text = "GO!";
        bottomSwapText.GetComponent<Text>().text = "GO!";

        yield return new WaitForSeconds(1f);
        topSwapText.GetComponent<Text>().text = "SWAP";
        bottomSwapText.GetComponent<Text>().text = "SWAP";
        topSwapText.SetActive(false);
        bottomSwapText.SetActive(false);
    }

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
    float getHit(bool inLettersTop)
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
        float hit = 0;
        int curIndex = getCurIndex(inLettersTop);
        switch (letter)
        {
            case "A":
                if (curIndex % 2 == 0)
                    hit = playerAInput.Action1;
                else
                    hit = playerBInput.Action1;
                break;
            case "B":
                if (curIndex % 2 == 0)
                    hit = playerAInput.Action2;
                else
                    hit = playerBInput.Action2;
                break;
            case "X":
                if (curIndex % 2 == 0)
                    hit = playerAInput.Action3;
                else
                    hit = playerBInput.Action3;
                break;
            case "Y":
                if (curIndex % 2 == 0)
                    hit = playerAInput.Action4;
                else
                    hit = playerBInput.Action4;
                break;
        }
        return hit;
    }
	
	// Update is called once per frame
	void Update () {
        // print("printing: " + inLettersBottom + " " + inLettersTop);
	    if (inLettersTop)
        {

            float hit = getHit(true);
            if (hit != 0)
            {
				// Show the player they correctly entered a part of the sequence
                topLetterList[curIndexTop].GetComponent<Image>().color = new Color32(60, 60, 60, 255);
                curIndexTop++;
				// If they finished the sequence clean up the GUI and do the powerup.
                if (curIndexTop >= letterListTop.Count)
                {
                    curIndexTop = 0;
                    inLettersTop = false;
					// Blank out the power up sequnce on the screen
                    for (int i = 0; i < topLetterList.Count; i++)
                    {
                        topLetterList[i].SetActive(false);
                        topLetterList[i].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    }

					PowerUp.ActivatePowerUp(true, topType);
					topType = powerUpType.empty;
                }
            }
        }
        if (inLettersBottom)
        {
            float hit = getHit(false);
            if (hit != 0)
            {
				// Show the player they correctly entered a part of the sequence
                bottomLetterList[curIndexBottom].GetComponent<Image>().color = new Color32(60, 60, 60, 255);
                curIndexBottom++;
				// If they finished the sequence clean up the GUI and do the powerup.
                if (curIndexBottom >= letterListBottom.Count)
                {
                    curIndexBottom = 0;
                    inLettersBottom = false;
					// Blank out the power up sequnce on the screen
                    for(int i = 0; i < bottomLetterList.Count; i++)
                    {
                        bottomLetterList[i].SetActive(false);
                        bottomLetterList[i].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    }

					PowerUp.ActivatePowerUp(false, bottomType);
					bottomType = powerUpType.empty;
                }
            } 
        }
	}

    public void hideGUI()
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

    public void showGUI()
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

	// Display power up sequence to the players 
    public void setLetters(bool isTopScreen, List<string> letters, powerUpType type)
    {
        if (!isTopScreen)
        {
            letterListTop = letters;
            for (int i = 0; i < topLetterList.Count; i++)
            {
                topLetterList[i].SetActive(true);
                topLetterList[i].GetComponent<Image>().sprite = getSpriteForLetter(letters[i]);
            }
            inLettersTop = true;
			topType = type;
        }
        else
        {
            letterListBottom = letters;
            for (int i = 0; i < bottomLetterList.Count; i++)
            {
                bottomLetterList[i].SetActive(true);
                bottomLetterList[i].GetComponent<Image>().sprite = getSpriteForLetter(letters[i]);
            }
            inLettersBottom = true;
			bottomType = type;
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
