using UnityEngine;
using System.Collections;
using UnityEngine.UI;
public class PauseScreen : MonoBehaviour {

    static public PauseScreen S;
    public GameObject text;
    public GameObject image;
    public float pauseStartTime;
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
        if (Time.time > 1)
            CarmonyGUI.S.hideGUI();
    }

    // Use this for initialization
    void Start () {
        //this doesnt work, not sure why
        Color newColor = GetComponent<Image>().color;
        newColor.a = 230;
        GetComponent<Image>().color = newColor;

        gameObject.SetActive(false);
        pauseStartTime = Time.time;
        Time.timeScale = 1;
        CarmonyGUI.S.showGUI();
    }

    // Update is called once per frame
    void Update () {
        if (Main.S.getStartPressed() && (Time.realtimeSinceStartup - pauseStartTime > .25))
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
}
