using UnityEngine;
using System.Collections;

public class GUI : MonoBehaviour {

    static public GUI S;
    public GameObject topGUI;
    public GameObject bottomGUI;
    public GameObject progressBar;
    public GameObject timeText;

    void Awake()
    {

        S = this;
    }

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void hideGUI()
    {
        topGUI.SetActive(false);
        bottomGUI.SetActive(false);
        progressBar.SetActive(false);
        timeText.SetActive(false);
    }

    public void showGUI()
    {
        topGUI.SetActive(true);
        bottomGUI.SetActive(true);
        progressBar.SetActive(true);
        timeText.SetActive(true);
    }
}
