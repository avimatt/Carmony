using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using InControl;

public class PracticeMap : MonoBehaviour {

    static public PracticeMap S;

    public bool carTopDone;
    public bool carBottomDone;

    public GameObject practiceText;
    public GameObject practiceText2;

    public GameObject topPlate;
    public GameObject bottomPlate;

    public bool carTopActive;
    public bool carBottomActive;

    bool shownHelpers;

    void Awake()
    {
        S = this;
    }

	// Use this for initialization
	void Start () {
        gameObject.SetActive(false);
	}

    IEnumerator showHelpers()
    {
        shownHelpers = true;
        CarmonyGUI.S.topPracticeHelper.SetActive(true);
        CarmonyGUI.S.bottomPracticeHelper.SetActive(true);
        yield return new WaitForSeconds(3);
        CarmonyGUI.S.topPracticeHelper.SetActive(false);
        CarmonyGUI.S.bottomPracticeHelper.SetActive(false);
    }

    // Update is called once per frame
    void Update () {
        if (Main.S.carTop.GetComponent<ArcadeVehicle>().first == 3 && Main.S.carTop.GetComponent<ArcadeVehicle>().second == 3)
        {
            carTopActive = false;
        }
        else
        {
            carTopActive = true;
        }
        if (Main.S.carBottom.GetComponent<ArcadeVehicle>().first == 3 && Main.S.carBottom.GetComponent<ArcadeVehicle>().second == 3)
        {
            carBottomActive = false;
        }
        else
        {
            carBottomActive = true;
        }
        if (!PauseScreen.S.isActiveAndEnabled)
        {
            //show swap helper in practice
            //if (!shownHelpers)
            //    StartCoroutine("showHelpers");

            practiceText.SetActive(true);
            practiceText2.SetActive(true);

            topPlate.SetActive(true);
            bottomPlate.SetActive(true);
        }
        for(int i = 0; i < InputManager.Devices.Count;i++)
        {
            InputDevice player = InputManager.Devices[i];
            if (player.RightBumper.WasPressed)
            {
                bool isFromTop = Main.S.isFromTopCar(i);
                if (isFromTop && !carTopDone)
                {
                    carTopDone = true;
                    topPlate.GetComponentInChildren<Text>().text = "READY";
                    if (!carBottomActive)
                    {
                        bottomPlate.GetComponentInChildren<Text>().text = "READY";
                        carBottomDone = true;
                    }
                    
                }else if (!isFromTop && !carBottomDone)
                {
                    carBottomDone = true;
                    bottomPlate.GetComponentInChildren<Text>().text = "READY";
                    if (!carTopActive)
                    {
                        topPlate.GetComponentInChildren<Text>().text = "READY";
                        carTopDone = true;
                    }
                }
            }
                
        }
        if (carBottomDone == true && carTopDone == true)
        {
            StartCoroutine("transportToStartCoroutine");
        }
	}

    //start sending cars to main course, disable practice UI elements
    void transportToStart()
    {
        Main.S.carTop.GetComponent<UserInteraction>().carrySpeed = 5;
        Main.S.carTop.GetComponent<UserInteraction>().moveToStart();
        Main.S.carBottom.GetComponent<UserInteraction>().carrySpeed = 5;
        Main.S.carBottom.GetComponent<UserInteraction>().moveToStart();
        Main.S.practicing = false;
        practiceText.SetActive(false);
        practiceText2.SetActive(false);
        topPlate.SetActive(false);
        bottomPlate.SetActive(false);
        CarmonyGUI.S.bottomPracticeHelper.SetActive(false);
        CarmonyGUI.S.topPracticeHelper.SetActive(false);
        CarmonyGUI.S.HideActivationButton();
        CarmonyGUI.S.movingToPractice.SetActive(true);
        Destroy(gameObject);
    }

    //wait a second before setting to main course
    IEnumerator transportToStartCoroutine()
    {
        //placeholder in case we want to wait
        yield return new WaitForSeconds(1);
        transportToStart();
    }


    //On finishing practice course, you are automatically readied up
    void OnTriggerEnter(Collider other)
    {
        //Check if it is a car that enters the checkpoint
        Transform playerTrans = null;
        Transform tmp = other.transform.parent;
        if (tmp)
        {
            playerTrans = tmp.transform.parent;
        }
        else
        {
            return;
        }

        CarState player = playerTrans.GetComponent<CarState>();
        if (playerTrans.GetComponent<UserInteraction>().isCarBottom)
        {
            bottomPlate.GetComponentInChildren<Text>().text = "READY";
            carBottomDone = true;
        }
        else
        {
            topPlate.GetComponentInChildren<Text>().text = "READY";
            carTopDone = true;
        }



    }

}
