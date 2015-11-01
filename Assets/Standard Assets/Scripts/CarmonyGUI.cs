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

    public GameObject topMinimap;
    public GameObject bottomMinimap;

    bool inLettersTop;
    bool inLettersBottom;
    int curIndex = 0;
    List<string> letterList;
    void Awake()
    {

        S = this;
    }

	// Use this for initialization
	void Start () {
        topMinimap.SetActive(false);
		topType = powerUpType.empty;
        bottomMinimap.SetActive(false);
		bottomType = powerUpType.empty;
	}

    float getHit(bool inLettersTop)
    {
		CarUserControl userContorl = inLettersTop ? Main.S.carTop.GetComponent<CarUserControl> () : Main.S.carBottom.GetComponent<CarUserControl> ();
		var playerAInput = InputManager.Devices[userContorl.first];
        var playerBInput = InputManager.Devices[userContorl.second];
        string letter = letterList[curIndex];
        float hit = 0;
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
	    if (inLettersTop)
        {

            float hit = getHit(true);
            if (hit != 0)
            {
                topLetterList[curIndex].GetComponent<Text>().color = new Color(34, 255, 0, 255);
                curIndex++;
				// If they finished the sequence clean up the GUI and do the powerup.
                if (curIndex > letterList.Count)
                {
                    curIndex = 0;
                    inLettersTop = false;
                    for (int i = 0; i < topLetterList.Count; i++)
                    {
                        topLetterList[i].GetComponent<Text>().text = "";
                        topLetterList[i].GetComponent<Text>().color = new Color(255, 255, 255, 255);
                    }

					PowerUp.ActivatePowerUp(true, topType);
					topType = powerUpType.empty;
                }
            }
        }
        else if (inLettersBottom)
        {
            float hit = getHit(false);
            if (hit != 0)
            {
                bottomLetterList[curIndex].GetComponent<Text>().color = new Color(34, 255, 0, 255);
                curIndex++;
				// If they finished the sequence clean up the GUI and do the powerup.
                if (curIndex >= letterList.Count)
                {
                    curIndex = 0;
                    inLettersBottom = false;
                    for(int i = 0; i < bottomLetterList.Count; i++)
                    {
                        bottomLetterList[i].GetComponent<Text>().text = "";
                        bottomLetterList[i].GetComponent<Text>().color = new Color(255, 255, 255, 255);
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
        timeText.SetActive(false);
        topLetters.SetActive(false);
        topMinimap.SetActive(false);
        bottomMinimap.SetActive(false);
    }

    public void showGUI()
    {
        topGUI.SetActive(true);
        bottomGUI.SetActive(true);
        timeText.SetActive(true);
        topLetters.SetActive(true);
        topMinimap.SetActive(true);
        bottomMinimap.SetActive(true);
    }

    public void setLetters(bool isTopScreen, List<string> letters, powerUpType type)
    {
        letterList = letters;
        if (!isTopScreen)
        {
            for(int i = 0; i < topLetterList.Count; i++)
            {
                topLetterList[i].GetComponent<Text>().text = letters[i];
            }
            inLettersTop = true;
			topType = type;
        }
        else
        {
            for (int i = 0; i < bottomLetterList.Count; i++)
            {
                bottomLetterList[i].GetComponent<Text>().text = letters[i];
            }
            inLettersBottom = true;
			bottomType = type;
        }
    }

    public void endGame(bool isTop)
    {
        if (isTop)
        {
            topEnd.SetActive(true);
            topEndTime.GetComponent<Text>().text = Timer.S.getGameTime();
            if (!Main.S.carBottomDone)
                topEndPlace.GetComponent<Text>().text = "1st";
            else
                topEndPlace.GetComponent<Text>().text = "2nd";
        }
        else
        {
            bottomEnd.SetActive(true);
            bottomEndTime.GetComponent<Text>().text = Timer.S.getGameTime();
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
