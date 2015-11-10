using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using InControl;

public class PauseScreen : MonoBehaviour {

    static public PauseScreen S;
    public GameObject text;
    public GameObject image;

    public List<GameObject> menuObjects;
    public int index;
    public float pauseStartTime;
    enum menuItems
    {
        resume,
        restart
    }
    void Awake()
    {
        S = this;
    }

    void OnEnable()
    {
        pauseStartTime = Time.realtimeSinceStartup;
        Time.timeScale = 0;
        image.SetActive(true);
        text.SetActive(true);
        foreach (GameObject go in menuObjects)
            go.SetActive(true);
        if (Time.time > 1 && (CarmonyGUI.S.topGUI || Main.S.practicing))
            CarmonyGUI.S.hideGUI();
    }

    // Use this for initialization
    void Start () {

        gameObject.SetActive(false);

        pauseStartTime = Time.time;
        Time.timeScale = 1;
        CarmonyGUI.S.showGUI();
    }

    void moveDownMenu()
    {
        index++;
        if (index >= menuObjects.Count)
            index = menuObjects.Count - 1;
    }

    void moveUpMenu()
    {
        index--;
        if (index < 0)
            index = 0;
    }

    // Update is called once per frame
    void Update () {
        if (YesNoMenu.S.isActiveAndEnabled)
            return;
        Color newColor = GetComponent<Image>().color;
        newColor.a = 230;
        GetComponent<Image>().color = newColor;
        foreach (InputDevice player in InputManager.Devices)
        {
            if (player.LeftStick.Left.WasPressed)
                moveUpMenu();
            else if (player.LeftStick.Right.WasPressed)
                moveDownMenu();
        }
        foreach (InputDevice player in InputManager.Devices)
        {
            if (player.Action1.WasPressed)
            {
                switch ((menuItems)index) {
                    case menuItems.resume:
                        resumeGame();
                        break;
                    case menuItems.restart:
                        YesNoMenu.S.gameObject.SetActive(true);
                        StartCoroutine("waitForMenu");
                        break;
                }
            }

        }
        for (int i = 0; i < menuObjects.Count; i++)
        {
            if (i == index)
            {
                menuObjects[i].GetComponent<Text>().color = new Color32(0, 255, 0, 255);
            }
            else
            {
                menuObjects[i].GetComponent<Text>().color = new Color32(255, 255, 255, 255);
            }
        }

        if (Main.S.getStartPressed() && (Time.realtimeSinceStartup - pauseStartTime > .25))
        {
            resumeGame();
        }
    }

    IEnumerator waitForMenu()
    {
        while (YesNoMenu.S.isActiveAndEnabled)
        {
            yield return 0;
        }
        if (YesNoMenu.S.result)
        {
            Application.LoadLevel("NoahDevScene");
        }
    }

    void resumeGame()
    {
        gameObject.SetActive(false);
        Main.S.updateInteractTimer();
        Main.S.paused = false;
        Time.timeScale = 1;
        text.SetActive(false);
        image.SetActive(false);
        CarmonyGUI.S.showGUI();
    }
}
