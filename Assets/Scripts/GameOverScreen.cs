using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class GameOverScreen : MonoBehaviour {

    public GameObject summaryPanel;
    public GameObject highscoresPanel;

    public Text numPerfect;
    public Text numPowHit;
    public Text numPowAct;
    public Text numResets;

    public List<Text> highscoreList;
    public bool isBottom;

    private CarState m_carstate;

    public float timer;
    public bool summaryShowing;
    void OnEnable()
    {
        if (isBottom)
        {
            m_carstate = Main.S.carBottom.GetComponent<CarState>();
        }
        else
        {
            m_carstate = Main.S.carTop.GetComponent<CarState>();
        }
        setSummary();
        setHighscoreList();

    }


    //Take summary values of car from carstate and set to text
    void setSummary()
    {
        numPerfect.text = "Perfects: " + m_carstate.numPerfectCheckpoints.ToString();
        numPowHit.text = "Powerups Hit: " + m_carstate.powerupsHit.ToString();
        numPowAct.text = "Powerups Used: " + m_carstate.powerupsActivated.ToString();
        numResets.text = "Resets: " + m_carstate.resets.ToString();
    }

    //take top n highscores from highscore object and display them in text
    void setHighscoreList()
    {
        for (int i = 0; i < highscoreList.Count; i++)
        {
            if (i >= HighScores.S.scoreList.Count)
            {
                highscoreList[i].text = "";
                highscoreList[i].color = new Color32(255, 255, 255, 255);
                continue;

            }
            highscoreList[i].text = HighScores.S.scoreList[i].getPlace() + " " + HighScores.S.scoreList[i].getName() + " " + HighScores.S.scoreList[i].getTime();
            if (HighScores.S.scoreList[i].getName() == m_carstate.name)
            {
                if (isBottom)
                {
                    highscoreList[i].color = HighScores.S.yellowCollor;
                }
                else
                {
                    highscoreList[i].color = HighScores.S.blueColor;
                }
            }
            else
            {
                highscoreList[i].color = new Color32(255, 255, 255, 255);
            }
        }
    }

	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    if (Time.time - timer > 10)
        {
            timer = Time.time;
            if (summaryShowing)
            {
                summaryPanel.SetActive(false);
                setHighscoreList();
                highscoresPanel.SetActive(true);
                summaryShowing = false;
            }
            else
            {
                highscoresPanel.SetActive(false);
                summaryPanel.SetActive(true);
                summaryShowing = true;
            }
        }
	}
}
