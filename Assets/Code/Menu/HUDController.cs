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
     
    public ModularInterface openInterface; 

    private void Start()
    {
        ConstructionMenu.SetActive(false);
        //ScienceMenu.SetActive(false);
        //MapMenu.SetActive(false);
    } 

    public bool OpenInterface(GameObject interfacePrefab, Entity entity, Vector3 screenPosition)
    {
        if(openInterface != null) { CloseInterface(); }

        openInterface = Instantiate(interfacePrefab, transform).GetComponent<ModularInterface>();
        openInterface.Init(entity, screenPosition + (Vector3.right * 20));

        return true;
    } 

    public void CloseInterface()
    {
        Destroy(openInterface.gameObject);
        openInterface = null;
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
     
    public void BulldozeButtonPressed()
    {
        GameManager.Instance.PlayerInputManager.ChangeInputState(PlayerInputManager.InputState.Demolish);
    }

    public void HeatmapButtonPressed() { } 


    // Viewport Swapping //

    public void ViewPortClicked()
    {
        GameManager.Instance.CameraController.SwapViews();
    }

}
