using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LapCounter : MonoBehaviour {
    public bool isTop;

    private Text m_text;
    private CarState m_carstate;

	// Use this for initialization
	void Start () {
        if (this.isTop)
        {
            m_carstate = Main.S.carTop.GetComponent<CarState>();
        }
        else
        {
            m_carstate = Main.S.carBottom.GetComponent<CarState>();
        }
        m_text = gameObject.GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update () {
        m_text.text = "Lap: " + m_carstate.currLap.ToString();
	}
}
