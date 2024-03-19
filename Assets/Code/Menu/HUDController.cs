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


    public MachineInterface activeMachineInterface;


    private void Start()
    {
        ConstructionMenu.SetActive(false);
        //ScienceMenu.SetActive(false);
        //MapMenu.SetActive(false);
    } 

    public bool OpenMachineInterface(Machine machine, Vector3 screenPosition)
    {
        if(activeMachineInterface != null) { CloseMachineInterface(); }

        activeMachineInterface = Instantiate(MenuData.Instance.MachineInterface, transform).GetComponent<MachineInterface>();
        activeMachineInterface.Init(machine, screenPosition + (Vector3.right * 20));

        return true;
    } 

    public void CloseMachineInterface()
    {
        Destroy(activeMachineInterface.gameObject);
        activeMachineInterface = null;
    } 

    // Menu Buttons //
     
    public void ConstructionMenuButtonPressed()
    {
        ToggleMenu(ConstructionMenu);
        ConstructionMenu.GetComponent<ConstructionMenuBuilder>().UpdateList();
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }  

    public void ScienceButtonPressed() { ToggleMenu(ScienceMenu); } 

    public void MapButtonPressed() { ToggleMenu(MapMenu); }

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


    // Construction Tools //

    public void CancelButtonPressed() { }

    public void BulldozeButtonPressed() { }

    public void HeatmapButtonPressed() { } 


    // Viewport Swapping //

    public void ViewPortClicked()
    {
        GameManager.Instance.cameraController.SwapViews();
    }

}
