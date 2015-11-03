using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LapCounter : MonoBehaviour {
    public bool isTop;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (this.isTop)
        {
            gameObject.GetComponent<Text>().text = "Lap: " + Main.S.carTop.GetComponent<CarState>().currLap.ToString();
        }
        else
        {
            gameObject.GetComponent<Text>().text = "Lap: " + Main.S.carBottom.GetComponent<CarState>().currLap.ToString();
        }
	}
}
