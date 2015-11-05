using UnityEngine;
using System;
using System.Collections;
using System.IO;
using System.Xml;

public enum Language {English, French}

public class LangManager
{
    private static LangManager instance;
    public static LangManager Instance { get { return instance ?? (instance = new LangManager()); } }

    private Language language;

    private Hashtable strings;

    public LangManager()
    {
        if (PlayerPrefs.HasKey("lang"))
        {
            string lang = PlayerPrefs.GetString("lang");

            switch(lang)
            {
                default:
                case "English": language = Language.English; break;
                case "French": language = Language.French; break;
            }
        }

        SetLanguage(language);
    }

    public string GetLanguage()
    {
        return language.ToString();
    }

    public void SetLanguage(string lang)
    {
        switch (lang)
        {
            default:
            case "English":
            case "en": language = Language.English; break;

            case "fr":
            case "French": language = Language.French; break;
        }

        SetLanguage(language); ;
    }

    public void SetLanguage(Language language)
    {
        string lang = language.ToString();
        PlayerPrefs.SetString("lang", lang);

        TextAsset textAsset = (TextAsset)Resources.Load("Lang/" + lang);

        var xml = new XmlDocument();
        xml.LoadXml(textAsset.text);
         
        strings = new Hashtable();
        var element = xml.DocumentElement[lang];
        if (element != null)
        {
            var elemEnum = element.GetEnumerator();
            while (elemEnum.MoveNext())
            {
                var xmlItem = (XmlElement)elemEnum.Current;
                strings.Add(xmlItem.GetAttribute("name"), xmlItem.InnerText);
            }
        } else {
            Debug.LogError("The specified language does not exist: " + language);
        }
    }

    public string GetString (string name)
    {
        if (!strings.ContainsKey(name))
        {
            Debug.LogError("The specified string does not exist: " + name);
             
            return "";
        }
     
        return (string)strings[name];
    }
     
}
