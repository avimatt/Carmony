using UnityEngine;
using System.Collections;
using InControl;
using System.Collections.Generic;
using UnityEngine.UI;

public class YesNoMenu : MonoBehaviour {

    static public YesNoMenu S;

    public List<GameObject> menuObjects;

    public bool result;
    public int index;

    enum options
    {
        yes,
        no
    }

    void Awake()
    {
        S = this;
    }

	// Use this for initialization
	void Start () {
        gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
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
                switch ((options)index)
                {
                    case options.yes:
                        result = true;
                        index = 0;
                        gameObject.SetActive(false);
                        break;
                    case options.no:
                        result = false;
                        index = 0;
                        gameObject.SetActive(false);
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
                menuObjects[i].GetComponent<Text>().color = new Color32(0, 0, 0, 255);
            }
        }
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
}
