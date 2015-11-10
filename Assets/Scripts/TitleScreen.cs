using UnityEngine;
using System.Collections;
using InControl;

public class TitleScreen : MonoBehaviour {

    static public TitleScreen S;

    void Awake()
    {
        S = this;
    }

	// Use this for initialization
	void Start () {
        //Time.timeScale = 0;

    }

    // Update is called once per frame
    void Update () {
        foreach (InputDevice player in InputManager.Devices)
        {
            if (player.AnyButton.WasPressed) { 
                gameObject.SetActive(false);
                StartScreen.S.gameObject.SetActive(true);
            }

        }
    }
}
