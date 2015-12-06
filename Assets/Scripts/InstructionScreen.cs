using UnityEngine;
using System.Collections;
using InControl;
using UnityEngine.UI;

public class InstructionScreen : MonoBehaviour {
	
	static public InstructionScreen S;
	
	public GameObject[] controlGroups = new GameObject[2];
	
	
	private bool firstGroup;
	private bool switchingGroups;

    public Text loadingText;
    public int loadIndex;
    bool inLoading;
    public Text continueText;
    bool instructionsDone;
    float startTime;
    bool switchingGroupsBack;
    bool manuallySwitched = true;
	void Awake()
	{
		S = this;
	}
	
	// Use this for initialization
	void Start () {
		gameObject.SetActive(false);
		firstGroup = true;
		switchingGroups = false;
        manuallySwitched = false;
	}
	
    IEnumerator loadingDots()
    {
        inLoading = true;
        while (gameObject.activeInHierarchy && !instructionsDone)
        {
            yield return new WaitForSeconds(.5f);
            string newText = "Loading";
            loadIndex++;
            if (loadIndex == 4)
                loadIndex = 0;
            for(int i = 0; i < loadIndex; i++)
            {
                newText += ".";
            }
            loadingText.text = newText;
            yield return 0;
        }
        inLoading = false;
    }

	// Update is called once per frame
	void Update()
	{
        if (!inLoading && !instructionsDone)
        {
            StartCoroutine("loadingDots");
            startTime = Time.time;
        }
        if (Time.time -startTime > 7 && firstGroup && !manuallySwitched)
        {
            switchingGroups = true;
            firstGroup = false;
        }
        else if (Time.time - startTime > 14 || manuallySwitched)
        {
            continueText.text = "Press any button to begin";
            loadingText.text = "";
            instructionsDone = true;
        }

		for (int i = 0; i < InputManager.Devices.Count; i++)
		{
            var player = InputManager.Devices[i];

            if (instructionsDone && player.AnyButton.WasPressed)
            {
                finishScreen();
            }
            // after both control groups have been seen
            if (player.MenuWasPressed )//&& !firstGroup && controlGroups[1].transform.localPosition[0] <= 0)
			{
                finishScreen();
			} 
			// after first control group shown
			else if(player.MenuWasPressed)
			{
				switchingGroups = true;
				firstGroup = false;
			}
            if (player.LeftStickX < 0)
            {
                if (!firstGroup)
                {
                    firstGroup = true;
                    switchingGroupsBack = true;
                    manuallySwitched = true;
                }
            }
            else if (player.LeftStickX > 0)
            {
                if (firstGroup)
                {
                    firstGroup = false;
                    switchingGroups = true;
                    manuallySwitched = true;
                }
            }
		}
		
		if (switchingGroups) {
			switchGroups ();
		}else if (switchingGroupsBack)
        {
            switchGroupsBack();
        }
	}

    void finishScreen()
    {
        Time.timeScale = 1;
        Main.S.practicing = true;
        gameObject.SetActive(false);
        PracticeMap.S.gameObject.SetActive(true);
        CarmonyMenuSystem.S.gameObject.SetActive(false);
        GameObject.Find("MainGameObject").GetComponent<AudioSource>().enabled = true;
    }
	
	void switchGroups(){
		if(controlGroups[1].transform.localPosition[0] > 0){
			Vector3 temp = controlGroups[0].transform.position;
			temp[0] -= 40;
			controlGroups[0].transform.position = temp;
			temp = controlGroups[1].transform.position;
			temp[0] -= 40;
			controlGroups[1].transform.position = temp;
		} else {
			switchingGroups = false;
		}
	}

    void switchGroupsBack()
    {
        if (controlGroups[0].transform.localPosition[0] < 0)
        {
            Vector3 temp = controlGroups[0].transform.position;
            temp[0] += 40;
            controlGroups[0].transform.position = temp;
            temp = controlGroups[1].transform.position;
            temp[0] += 40;
            controlGroups[1].transform.position = temp;
        }
        else
        {
            switchingGroupsBack = false;
        }
    }
	
}
