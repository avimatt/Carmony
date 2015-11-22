using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Text;
using System;
using UnityEngine.UI;
using InControl;

public class scoreRow
{

    int place;
    string name;
    int minutes;
    int seconds;
    public int getPlace()
    {
        return place;
    }
    public void setPlace(int _place)
    {
        place = _place;
    }
    public string getName()
    {
        return name;
    }
    public void setName(string _name)
    {
        name = _name;
    }
    public string getTime()
    {
        string time = minutes.ToString();
        if (seconds < 10)
        {
            time += ":0";
        }
        else
        {
            time += ": ";
        }
        time += seconds.ToString();
        return time;
    }
    public int getMinutes()
    {
        return minutes;
    }
    public int getSeconds()
    {
        return seconds;
    }
    public void setTime(int _minutes,int _seconds)
    {
        minutes = _minutes;
        seconds = _seconds;
    }
}

public class HighScores : MonoBehaviour {

    static public HighScores S;
    private string filename = "highScores.txt";
    public List<scoreRow> scoreList = new List<scoreRow>();
    public List<Text> places;
    public List<Text> names;
    public List<Text> times;

    public Color32 blueColor;
    public Color32 yellowCollor;
    public bool recordBeaten;
    void Awake()
    {
        S = this;
    }

    void OnEnable()
    {
        if (Main.S)
            readFile();
    }

	// Use this for initialization
	void Start () {
        readFile();
        gameObject.SetActive(false);
    }
	
    //Update Text in the GUI to show TopScores
    public void updateText()
    {
        int i = 0;
        for (; i < scoreList.Count; i++)
        {
            places[i].text = scoreList[i].getPlace().ToString();
            names[i].text = scoreList[i].getName();
            times[i].text = scoreList[i].getTime();
            if (scoreList[i].getName() == Main.S.carTop.GetComponent<CarState>().name)
            {
                places[i].color = blueColor;
                names[i].color = blueColor;
                times[i].color = blueColor;
            }
            else if (scoreList[i].getName() == Main.S.carBottom.GetComponent<CarState>().name)
            {
                places[i].color = yellowCollor;
                names[i].color = yellowCollor;
                times[i].color = yellowCollor;
            }
            else
            {
                places[i].color = new Color32(255, 255, 255, 255);
                names[i].color = new Color32(255, 255, 255, 255);
                times[i].color = new Color32(255, 255, 255, 255);
            }
        }
        for (; i < places.Count; i++)
        {
            places[i].text = "";
            names[i].text = "";
            times[i].text = "";
        }
    }

	// Update is called once per frame
	void Update () {
        updateText();
        foreach (InputDevice player in InputManager.Devices)
        {
            if (player.Action2.WasPressed && !Main.S.getRaceStarted())
            {
                emptyTexts();
                gameObject.SetActive(false);
            }else if (player.AnyButton.WasPressed && Main.S.getRaceStarted()){
                Application.LoadLevel("NoahDevScene");
            }
        }
    }

    void emptyTexts()
    {
        for (int i = 0; i < places.Count; i++)
        {
            places[i].text = "";
            names[i].text = "";
            times[i].text = "";
        }
    }

    void readFile()
    {
        scoreList.Clear();
        StreamReader theReader = new StreamReader(Main.S.Map.GetComponent<Map>().name + filename, Encoding.Default);
        using (theReader)
        {
            string line;
            do
            {
                line = theReader.ReadLine();
                if (line != null && line != "")
                {
                    string[] entries = line.Split(' ');
                    scoreRow newRow = new scoreRow();
                    newRow.setPlace(Int32.Parse(entries[0]));
                    newRow.setName(entries[1]);
                    newRow.setTime(Int32.Parse(entries[2]), Int32.Parse(entries[3]));
                    scoreList.Add(newRow);
                }
            } while (line != null);
        }
    }

    void writeFile()
    {
        StreamWriter sw;
        sw = File.CreateText(Main.S.Map.GetComponent<Map>().name + filename);
        print("count " + scoreList.Count + scoreList[0].getName());
        for(int i = 0; i < scoreList.Count; i++)
        {
            sw.WriteLine(scoreList[i].getPlace().ToString()+ " " +  scoreList[i].getName()+ " " + scoreList[i].getMinutes().ToString() + " " +  scoreList[i].getSeconds().ToString());
        }
        sw.Close();
    }

    public bool updateList(scoreRow newRow)
    {
        int myTime = 60*newRow.getMinutes() +  newRow.getSeconds();
        bool inserted = false;
        int i = 0;
        for(; i < scoreList.Count; i++)
        {
            if (!inserted)
            {
                int curTime = 60* scoreList[i].getMinutes() + scoreList[i].getSeconds();
                if (myTime <= curTime)
                {
                    newRow.setPlace(scoreList[i].getPlace());
                    scoreList.Insert(i, newRow);
                    inserted = true;
                    i++;
                    break;
                }
            }
        }

        for (;i < scoreList.Count; i++)
        {
            int curTime = 60*scoreList[i].getMinutes() + scoreList[i].getSeconds();
            int lastTime = 60 * scoreList[i-1].getMinutes() + scoreList[i-1].getSeconds();

            if (curTime == lastTime)
            {
                scoreList[i].setPlace(scoreList[i - 1].getPlace());
            }
            else
            {
                scoreList[i].setPlace(i+1);
            }
        }

        if (scoreList.Count == 0)
        {
            newRow.setPlace(1);
            scoreList.Insert(i, newRow);
            inserted = true;
        }
        if (scoreList.Count < places.Count && !inserted)
        {
            int lastTime = 60 * scoreList[scoreList.Count -1].getMinutes() + scoreList[scoreList.Count - 1].getSeconds();
            if (myTime == lastTime)
            {
                newRow.setPlace(scoreList[scoreList.Count - 1].getPlace());
            }
            else
            {
                newRow.setPlace(scoreList.Count + 1);
            }
            scoreList.Add(newRow);
            inserted = true;
        }
        if (inserted)
        {
            if (scoreList.Count > places.Count)
            {
                print(places.Count + " " +  (scoreList.Count - places.Count));
                scoreList.RemoveRange(places.Count, scoreList.Count - places.Count );
            }
            print("re writing file " + scoreList.Count);
            recordBeaten = true;
            writeFile();
            return true;
        }
        return false;
    }
}
