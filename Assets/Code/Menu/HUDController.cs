using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private GameObject BuildMenu;

    [SerializeField] private GameObject ScienceMenu; 

    [SerializeField] private GameObject MapMenu;


    private void Start()
    {
        BuildMenu.SetActive(false);
        //ScienceMenu.SetActive(false);
        //MapMenu.SetActive(false);
    }

    public void BuildMenuButtonPressed()
    {
        ToggleMenu(BuildMenu);
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
