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


    private GameObject activeInterface;


    private void Start()
    {
        ConstructionMenu.SetActive(false);
        //ScienceMenu.SetActive(false);
        //MapMenu.SetActive(false);
    } 

    public void OpenMachineInterface(Machine machine, Vector3 screenPosition)
    {
        if(activeInterface != null) { CloseActiveInterface(); }

        activeInterface = Instantiate(GlobalData.Instance.MachineInterface, transform);
        activeInterface.GetComponent<MachineInterface>().Init(machine, screenPosition + (Vector3.right * 20));
    } 

    public void CloseActiveInterface()
    {
        Destroy(activeInterface);
        activeInterface = null;
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

}
