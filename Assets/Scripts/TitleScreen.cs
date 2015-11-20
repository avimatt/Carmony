using UnityEngine;
using System.Collections;
using InControl;
using UnityEngine.UI;
using System.Collections.Generic;


public enum difficultyLevel
{
    easy,
    medium,
    hard
}
public class TitleScreen : MonoBehaviour {

    static public TitleScreen S;
    public int index;
    public List<Text> menuObjects;
    public difficultyLevel mapDifficulty;
    void Awake()
    {
        S = this;
    }

	// Use this for initialization
	void Start () {
        //Time.timeScale = 0;

    }

    // Update is called once per frame
    void Update()
    {
        if (HighScores.S.isActiveAndEnabled)
            return;
        foreach (InputDevice player in InputManager.Devices)
        {
            if (player.LeftStick.Up.WasPressed)
                moveUpMenu();
            else if (player.LeftStick.Down.WasPressed)
                moveDownMenu();
        }
        for (int i = 0; i < menuObjects.Count; i++)
        {
            if (i == index)
            {
                menuObjects[i].color = new Color32(0, 0, 0, 255);
            }
            else
            {
                menuObjects[i].color = new Color32(54, 54, 54, 255);
            }
        }
        foreach (InputDevice player in InputManager.Devices)
        {
            if (player.Action1.WasPressed)
            {
                print("index: " + index);
                switch (index)
                {
                    case 0:
                        gameObject.SetActive(false);
                        chooseMap();
                        StartScreen.S.gameObject.SetActive(true);
                        break;
                    case 1:
                        HighScores.S.gameObject.SetActive(true);
                        break;
                    case 2:
                        changeMapDifficulty(menuObjects[2]);
                        break;
                }
            }
        }
    }


    void chooseMap()
    {
        switch (mapDifficulty)
        {
            case difficultyLevel.easy:
                Main.S.Map = Main.S.MapList[1];
                break;
            case difficultyLevel.medium:
                Main.S.Map = Main.S.MapList[0];
                break;
            case difficultyLevel.hard:
                Main.S.Map = Main.S.MapList[3];
                break;
        }
    }
    void changeMapDifficulty(Text difficultyText)
    {
        switch (mapDifficulty)
        {
            case difficultyLevel.easy:
                mapDifficulty = difficultyLevel.medium;
                difficultyText.text = "Difficulty: Medium";
                Main.S.Map = Main.S.MapList[1];
                break;
            case difficultyLevel.medium:
                mapDifficulty = difficultyLevel.hard;
                difficultyText.text = "Difficulty: Hard";
                Main.S.Map = Main.S.MapList[0];

                break;
            case difficultyLevel.hard:
                mapDifficulty = difficultyLevel.easy;
                difficultyText.text = "Difficulty: Easy";
                Main.S.Map = Main.S.MapList[3];

                break;
        }
    }
    void moveDownMenu()
    {
        index++;
        if (index >= menuObjects.Count)
            index = menuObjects.Count - 1;
        print("new index: " + index);
    }

    void moveUpMenu()
    {
        index--;
        if (index < 0)
            index = 0;
        print("new index: " + index);

    }
}
