using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityStandardAssets.Vehicles.Car;

public class Speed : MonoBehaviour {
    public bool isTop;
    public Text m_text;
    public CarController m_car;
	// Use this for initialization
	void Start () {
        m_text = gameObject.GetComponent<Text>();
        if (isTop)
        {
            m_car = Main.S.carTop.GetComponent<CarController>();
        }
        else
        {
            m_car = Main.S.carBottom.GetComponent<CarController>();
        }
    }
	
	// Update is called once per frame
	void Update () {
        //comment in for normal text
        // m_text.text = m_car.getSpeed().ToString() + " MPH";
        m_text.text = m_car.getSpeed().ToString();
    }
}
