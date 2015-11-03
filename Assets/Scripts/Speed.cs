using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class Speed : MonoBehaviour {
    public bool isTop;
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	    if (isTop)
        {
            gameObject.GetComponent<Text>().text = Main.S.carTop.GetComponent<CarController>().getSpeed().ToString() + " MPH";
        }
        else
        {
            gameObject.GetComponent<Text>().text = Main.S.carBottom.GetComponent<CarController>().getSpeed().ToString() + " MPH";
        }
    }
}
