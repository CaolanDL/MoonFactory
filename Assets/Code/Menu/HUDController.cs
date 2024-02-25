using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDController : MonoBehaviour
{
    [SerializeField] private GameObject BuildMenu;
    private GameObject activeBuildMenu;

    [SerializeField] private GameObject ScienceMenu;
    private GameObject activeScienceMenu;

    [SerializeField] private GameObject MapMenu;
    private GameObject activeMapMenu;

    public void BuildButtonPressed()
    {
        if(activeBuildMenu != null) { return; }

        activeBuildMenu = Instantiate(BuildMenu);
    }

    public void CancelButtonPressed()
    {

    }

    public void BulldozeButtonPressed()
    {

    }


    public void ScienceButtonPressed()
    {
        if (activeScienceMenu != null) { return; }

        activeScienceMenu = Instantiate(ScienceMenu);
    }


    public void HeatmapButtonPressed()
    {

    }

    public void MapButtonPressed()
    {
        if (activeMapMenu != null) { return; }

        activeMapMenu = Instantiate(MapMenu);
    }
}
