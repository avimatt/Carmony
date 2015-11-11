using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class Logger : MonoBehaviour {

    static public Logger S;

    string fileNameTop = "trackRecordTop.txt";
    string fileNameBottom = "trackRecordBottom.txt";
    StreamWriter swTop;
    StreamWriter swBottom;
    public bool noPrint;
    void Awake()
    {
        S = this;
    }

    public void writeFile(bool isTop,string message)
    {
        if (noPrint) return;
        if (isTop)
            swTop.WriteLine(message);
        else
            swBottom.WriteLine(message);
    }

    public void printSummary(bool isTop)
    {
        if (noPrint) return;

        if (isTop)
        {
            CarState carTop = Main.S.carTop.GetComponent<CarState>();
            writeFile(true, "");
            writeFile(true, "");
            writeFile(true, "");
            writeFile(true, "");
            writeFile(true, "Summary:");
            writeFile(true, "Perfect Checkpoints: " + carTop.numPerfectCheckpoints);
            writeFile(true, "Total Time: " + carTop.totalTime);
            writeFile(true, "Powerups Hit: " + carTop.powerupsHit);
            writeFile(true, "Powerups Activated: " + carTop.powerupsActivated);
            writeFile(true, "Resets: " + carTop.resets);
        }
        else
        {
            CarState carBottom = Main.S.carBottom.GetComponent<CarState>();
            writeFile(false, "");
            writeFile(false, "");
            writeFile(false, "");
            writeFile(false, "");
            writeFile(false, "Summary:");
            writeFile(false, "Perfect Checkpoints: " + carBottom.numPerfectCheckpoints);
            writeFile(false, "Total Time: " + carBottom.totalTime);
            writeFile(false, "Powerups Hit: " + carBottom.powerupsHit);
            writeFile(false, "Powerups Activated: " + carBottom.powerupsActivated);
            writeFile(false, "Resets: " + carBottom.resets);
        }
    }

    void OnDestroy()
    {
		if (noPrint) return;

        swTop.Close();
        swBottom.Close();
    }


    // Use this for initialization
    void Start () {
        if (noPrint) return;

        int i = 1;
        while (File.Exists(fileNameTop))
        {
            print(File.Exists(fileNameTop) + "   " + fileNameTop);
            fileNameTop = "trackRecordTop" + i + ".txt";
            i++;
        }
        swTop = File.CreateText(fileNameTop);
        i = 1;
        while (File.Exists(fileNameBottom))
        {
            print(File.Exists(fileNameBottom) + "   " + fileNameBottom);
            fileNameBottom = "trackRecordBottom" + i + ".txt";
            i++;
        }
        swBottom = File.CreateText(fileNameBottom);
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
