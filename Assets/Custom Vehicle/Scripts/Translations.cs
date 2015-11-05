using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class Translations : MonoBehaviour
{
    public List<Text> fields = null;

    void Start()
    {
        UpateFields();
    }

    public void UpateFields()
    {
        foreach(Text field in fields)
        {
            if (field != null)
            {
                field.text = LangManager.Instance.GetString(field.gameObject.name);
            }
            //else Debug.Log("Missing field to translate");
        }
    }
}
