using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenu : MonoBehaviour
{
    public Menu menu;
    public UIDropdown menuLanguages;
    public Translations translations;

    public List<Car> cars = new List<Car>();

	void Start ()
    {
        // Let's make all the cars in the menu uncontrollable by the keyboard
        foreach(Car car in cars)
        {
            car.isControllable = false;
            car.StopSounds();
        }

        menu.ShowMenuPage("MainMenu");

        string lang = LangManager.Instance.GetLanguage();
        menuLanguages.SetValues(lang, lang);
        menuLanguages.transform.parent.gameObject.SetActive(true);
        menuLanguages.OnChange(this.gameObject, "SetLanguage");
	}

    public void SetLanguage(string lang)
    {
        LangManager.Instance.SetLanguage(lang);
        translations.UpateFields();
    }

    public void GoToScene(string scene)
    {
        Application.LoadLevel(scene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
