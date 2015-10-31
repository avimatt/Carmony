﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using InControl;

public class CarmonyGUI : MonoBehaviour {

    static public CarmonyGUI S;
    public GameObject topGUI;
    public GameObject bottomGUI;
    public GameObject progressBar;
    public GameObject timeText;
    public GameObject topLetters;
    public List<GameObject> topLetterList;
    public GameObject bottomLetters;
    public List<GameObject> bottomLetterList;

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
	
	}

    float getHit(bool inLettersTop)
    {
        var playerAInput = InputManager.Devices[0];
        var playerBInput = InputManager.Devices[1];
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
                if (curIndex > letterList.Count)
                {
                    curIndex = 0;
                    inLettersTop = false;
                    for (int i = 0; i < topLetterList.Count; i++)
                    {
                        topLetterList[i].GetComponent<Text>().text = "";
                        topLetterList[i].GetComponent<Text>().color = new Color(255, 255, 255, 255);

                    }
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
                if (curIndex >= letterList.Count)
                {
                    curIndex = 0;
                    inLettersBottom = false;
                    for(int i = 0; i < bottomLetterList.Count; i++)
                    {
                        bottomLetterList[i].GetComponent<Text>().text = "";
                        bottomLetterList[i].GetComponent<Text>().color = new Color(255, 255, 255, 255);

                    }
                }
            }
            
        }
	}

    public void hideGUI()
    {
        topGUI.SetActive(false);
        bottomGUI.SetActive(false);
        progressBar.SetActive(false);
        timeText.SetActive(false);
        topLetters.SetActive(false);
    }

    public void showGUI()
    {
        topGUI.SetActive(true);
        bottomGUI.SetActive(true);
        progressBar.SetActive(true);
        timeText.SetActive(true);
        topLetters.SetActive(true);
    }

    public void setLetters(bool isTopScreen, List<string> letters)
    {
        letterList = letters;
        if (!isTopScreen)
        {
            for(int i = 0; i < topLetterList.Count; i++)
            {
                topLetterList[i].GetComponent<Text>().text = letters[i];
            }
            inLettersTop = true;
        }
        else
        {
            for (int i = 0; i < bottomLetterList.Count; i++)
            {
                bottomLetterList[i].GetComponent<Text>().text = letters[i];
            }
            inLettersBottom = true;
        }
    }
}
