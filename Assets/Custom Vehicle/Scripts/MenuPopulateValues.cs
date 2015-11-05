using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuPopulateValues : MonoBehaviour
{
    public UIDropdown dropdownResolution;
    public UIDropdown dropdownGraphicsQualitySettings;
    public UIDropdown dropdownBloomEffect;
    public UIDropdown dropdownReflections;

    public Toggle toggleFullscreen;
    public Toggle toggleSSAO;

    void Start()
    {
        Resolution res;

        // Add resolutions list (higher ones on top)
        for(int i = 0; i < Screen.resolutions.Length; i++)
        {
            res = Screen.resolutions[Screen.resolutions.Length-i-1];

            dropdownResolution.AddOption(res.width + " x " + res.height, res.width + "x" + res.height);
        }

        dropdownResolution.SetValues(Screen.width + " x " + Screen.height, Screen.width + "x" + Screen.height);

        // Populate Graphics Quality Dropdown (higher quality on top)
        int qualityLevelIndex = 0;
        for (int i = 0; i < QualitySettings.names.Length; i++)
        {
            qualityLevelIndex = QualitySettings.names.Length - i - 1;
            string quality = QualitySettings.names[qualityLevelIndex];
            
            dropdownGraphicsQualitySettings.AddOption(quality, qualityLevelIndex.ToString());
        }

        int indexQualityLevel = QualitySettings.GetQualityLevel();
        string qualityName = QualitySettings.names[indexQualityLevel];
        dropdownGraphicsQualitySettings.SetValues(qualityName, indexQualityLevel.ToString());

        // Tell the Fullscreen toggle if we are in fullscreen mode
        toggleFullscreen.isOn = Screen.fullScreen;

        // Bloom effect
        string bloomEffect = "high";

        if (PlayerPrefs.HasKey("bloomEffect"))
            bloomEffect = PlayerPrefs.GetString("bloomEffect");

        dropdownBloomEffect.SetValues(LangManager.Instance.GetString(bloomEffect), bloomEffect);

        // SSAO effect
        bool useSSAO = true;

        if (PlayerPrefs.HasKey("SSAOEffect"))
            useSSAO = PlayerPrefs.GetInt("SSAOEffect") > 0;

        toggleSSAO.isOn = useSSAO;

        // Reflections
        string reflectionsQuality = "high";

        if (PlayerPrefs.HasKey("reflectionsQuality"))
            reflectionsQuality = PlayerPrefs.GetString("reflectionsQuality");

        dropdownReflections.SetValues(LangManager.Instance.GetString(reflectionsQuality), reflectionsQuality);
    }
}
