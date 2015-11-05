using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIBtnDropdown : MonoBehaviour, IPointerClickHandler, ISubmitHandler
{
    public UIDropdown dropdown;
    public Text text;
    public string value;

    public void OnPointerClick(PointerEventData ped)
    {
        Select();
    }

    public void OnSubmit(BaseEventData eventData)
    {
        Select();
    }

    public void Select()
    {
        dropdown.Select(this);
        EventSystem.current.SetSelectedGameObject(dropdown.gameObject);
    }
}
