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
                    CarmonyGUI.S.bottomMinimap.SetActive(true);
                    CarmonyGUI.S.topMinimap.SetActive(true);
					CarmonyGUI.S.topMinimapDots.SetActive(true);
					CarmonyGUI.S.bottomMinimapDots.SetActive(true);
                    CarmonyGUI.S.bottomImageLeft.SetActive(true);
                    CarmonyGUI.S.bottomImageRight.SetActive(true);
                    CarmonyGUI.S.topImageLeft.SetActive(true);
                    CarmonyGUI.S.topImageRight.SetActive(true);
                    CarmonyGUI.S.raiseStartFlagText();                    
                    GameObject.Find("MainGameObject").GetComponent<AudioSource>().enabled = true;
                }
            }
        }
    }
}
