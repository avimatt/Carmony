using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public RectTransform[] menus;

    private Dictionary<string, RectTransform> m_menus;
    private List<string> m_pagesHistory = new List<string>();

    void Awake()
    {
        m_menus = new Dictionary<string, RectTransform>();

        foreach (RectTransform t in menus)
        {
            string key = t.gameObject.name;

            if (!m_menus.ContainsKey(key))
            {
                m_menus.Add(key, t);
            }
        }
    }

    public bool IsMenuActive(string page)
    {
        if (m_menus.ContainsKey(page))
        {
            RectTransform menu = m_menus[page];

            if (menu.gameObject.activeSelf)
                return true;
        }

        return false;
    }

    public void HideMenu()
    {
        foreach (RectTransform menu in m_menus.Values)
        {
            menu.gameObject.SetActive(false);
        }
    }

    public void ClearHistory()
    {
        if (m_pagesHistory.Count > 1)
            m_pagesHistory.Clear();
    }

    // Move menu to the previous panel
    public void GoBack()
    {
        m_pagesHistory.RemoveAt(m_pagesHistory.Count - 1);

        if (m_pagesHistory.Count > 0)
        {
            ShowMenuPage(m_pagesHistory[m_pagesHistory.Count - 1]);
            m_pagesHistory.RemoveAt(m_pagesHistory.Count - 1);
        }
    }

    public void ShowMenuPage(string page)
    {
        // first close all open menus
        HideMenu();

        // then find the chosen menu and show it
        if (m_menus.ContainsKey(page))
        {
            m_pagesHistory.Add(page);

            RectTransform menu = m_menus[page];
            menu.gameObject.SetActive(true);

            // Let's find the first button on that menu panel and focus on it
            for (int i = 0; i < menu.childCount; i++)
            {
                if (menu.GetChild(i).GetComponent<Button>() != null)
                {
                    EventSystem.current.SetSelectedGameObject(menu.GetChild(i).gameObject);
                    break;
                }
            }
        }
    }
}
