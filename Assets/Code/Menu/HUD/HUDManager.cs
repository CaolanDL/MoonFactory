using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] public GameObject ConstructionMenu; 
    [SerializeField] public GameObject TechTree;
    [SerializeField] public GameObject MapMenu;

    [SerializeField] public MouseIconManager MouseIconManager;

    [SerializeField] Transform interfaceParent;
    public StaticInterface openInterface;

    private void Start()
    {
        ConstructionMenu.SetActive(false);
        TechTree.SetActive(false);
        //MapMenu.SetActive(false);
    } 

    public bool OpenInterface(GameObject interfacePrefab, Entity entity, Vector3 screenPosition)
    {
        if(openInterface != null) { CloseInterface(); }

        openInterface = Instantiate(interfacePrefab, interfaceParent).GetComponent<StaticInterface>();
        openInterface.Init(entity, screenPosition + (Vector3.right * 20));

        return true;
    } 

    public void CloseInterface()
    {
        Destroy(openInterface.gameObject);
        openInterface = null;
    } 

    // Menu Buttons //
     
    public void ToggleBuildMenu()
    {
        ToggleMenu(ConstructionMenu);
        ConstructionMenu.GetComponent<ConstructionMenuBuilder>().UpdateList();
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }  

    public void ToggleScienceMenu() { ToggleMenu(TechTree); } 

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
     
    public void BulldozeButtonPressed()
    {
        GameManager.Instance.PlayerInputManager.ChangeInputState(PlayerInputManager.InputState.Demolish);
    }

    public void ElectricalCoverageButtonPressed()
    {
        ElectricalCoverageRenderer.enabled = !ElectricalCoverageRenderer.enabled;

        // Enable electrical overlay post process
    }

    public void HeatmapButtonPressed() { } 


    // Viewport Swapping //

    public void ViewPortClicked()
    {
        GameManager.Instance.CameraController.SwapViews();
    }

}
