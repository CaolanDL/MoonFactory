using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HUDController : MonoBehaviour
{
    [SerializeField] private GameObject ConstructionMenu;

    [SerializeField] private GameObject ScienceMenu; 

    [SerializeField] private GameObject MapMenu;


    private void Start()
    {
        ConstructionMenu.SetActive(false);
        //ScienceMenu.SetActive(false);
        //MapMenu.SetActive(false);
    }

    public void ConstructionMenuButtonPressed()
    {
        ToggleMenu(ConstructionMenu);
        ConstructionMenu.GetComponent<ConstructionMenuBuilder>().UpdateList();
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void CancelButtonPressed()
    {

    }

    public void BulldozeButtonPressed()
    {

    }


    public void ScienceButtonPressed()
    {
        ToggleMenu(ScienceMenu);
    }


    public void HeatmapButtonPressed()
    {

    }

    public void MapButtonPressed()
    {
        ToggleMenu(MapMenu); 
    }

    public void ToggleMenu(GameObject menu)
    {
        if (menu == null) { return; } 

        if (menu.activeInHierarchy)
        {
            menu.SetActive(false); 
        }
        else
        {
            menu.SetActive(true); 
        }
    }
}
