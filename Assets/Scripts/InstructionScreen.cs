using UnityEngine;
using System.Collections;
using InControl;

public class InstructionScreen : MonoBehaviour {

    static public InstructionScreen S;

    void Awake()
    {
        S = this;
    }

	// Use this for initialization
	void Start () {
        gameObject.SetActive(false);
	}

    // Update is called once per frame
    void Update()
    {
        if (Time.realtimeSinceStartup- StartScreen.S.cooldown  > .2)
        {
            for (int i = 0; i < InputManager.Devices.Count; i++)
            {
                var player = InputManager.Devices[i];
                if (player.AnyButton)
                {
                    Time.timeScale = 1;
                    gameObject.SetActive(false);
                    CarmonyGUI.S.raiseStartFlagText();
                    GameObject.Find("MainGameObject").GetComponent<AudioSource>().enabled = true;
                }
            }
        }
    }
}
