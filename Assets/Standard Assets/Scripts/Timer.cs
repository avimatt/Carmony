using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

    static public Timer S;

    void Awake()
    {
        S = this;
    }
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        gameObject.GetComponent<Text>().text = getGameTime();
	}


    public string getGameTime()
    {
        int minutes = (int)(Time.time / 60);
        int seconds = (int)(Time.time % 60);
        string secondString = seconds.ToString();
        if (seconds < 10)
            secondString = "0" + seconds.ToString();
        return minutes + ":" + secondString;
    }
}
