using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class UIDropdown : MonoBehaviour
{
    public string value;
    public UIDropdownPanel panelDropdownContent;
    public Text text;
    public RectTransform content;

    public GameObject BtnDropdownPrefab;

    private GameObject m_onChangeObj;
    private string m_onChangeStr;

    private RectTransform m_rectTransform;
    private RectTransform m_LastBtnSelected;

    void Start()
    {
        m_rectTransform = (RectTransform)transform;

        content.gameObject.SetActive(false);

        Navigation customNav = new Navigation();
        customNav.mode = Navigation.Mode.Explicit;

        // Creating navigation for the dropdown buttons
        List<Button> btns = new List<Button>();

        foreach (RectTransform tr in content)
        {
            Button btn = tr.gameObject.GetComponent<Button>();
            btns.Add(btn);
        }

        for (int i = 0; i < btns.Count; i++)
        {
            if (i > 0)
                customNav.selectOnUp = btns[i-1];

            if (i < btns.Count-1)
                customNav.selectOnDown = btns[i+1];
            
            btns[i].navigation = customNav;
        }

        if (btns.Count > 0)
            m_LastBtnSelected = (RectTransform)btns[0].transform;
    }

    public void SetValues(string name, string val)
    {
        value = val;
        text.text = name;
    }

    public void Select(UIBtnDropdown btn)
    {
        SetValues(btn.text.text, btn.value);

        m_LastBtnSelected = (RectTransform)btn.gameObject.transform;

        HideContent();

        if (m_onChangeObj != null && value.Length > 0)
            m_onChangeObj.SendMessage(m_onChangeStr, value, SendMessageOptions.RequireReceiver);
    }

    public void HideContent()
    {
        content.SetParent(transform);
        content.gameObject.SetActive(false);
    }

    public void ShowContent()
    {
        if (content.parent != transform)
        {
            HideContent();
            return;
        }

        content.gameObject.SetActive(true);
        content.SetParent(panelDropdownContent.gameObject.transform);


        if (panelDropdownContent.currentDropdown != null && panelDropdownContent.currentDropdown != this)
            panelDropdownContent.currentDropdown.HideContent();

        panelDropdownContent.currentDropdown = this;

        Vector2 pos = m_rectTransform.position;
        pos.y -= m_LastBtnSelected.localPosition.y;
        content.position = new Vector2(content.position.x, pos.y);

        EventSystem.current.SetSelectedGameObject(m_LastBtnSelected.gameObject);
    }

    public void OnChange(GameObject obj, string strCallback)
    {
        m_onChangeObj = obj;
        m_onChangeStr = strCallback;
    }

    public void AddOption(string name, string value)
    {
        GameObject btn = Instantiate(BtnDropdownPrefab) as GameObject;
        btn.transform.SetParent(content);
        btn.transform.localScale = new Vector3(1, 1, 1);

        // Select first element added
        if (content.childCount == 1)
            m_LastBtnSelected = (RectTransform)btn.transform;

        UIBtnDropdown button = btn.GetComponent<UIBtnDropdown>();
        button.dropdown = this;
        button.text.text = name;
        button.value = value;
    }
}
