using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    [SerializeField] public GameObject OptionsMenu;

    [SerializeField] public GameObject ConstructionMenu;
    [SerializeField] public GameObject TechTree;
    [SerializeField] public GameObject MapMenu;

    public Transform WorldIconContainer;

    [SerializeField] public MouseIconManager MouseIconManager;

    [SerializeField] Transform interfaceParent;
    public StaticInterface openInterface;

    private void Start()
    {
        ConstructionMenu.GetComponent<PopOutMenu>()?.SetState(true);
        TechTree.GetComponent<PopOutMenu>()?.SetState(true);
    }

    public bool OpenInterface(GameObject interfacePrefab, Entity entity, Vector3 screenPosition)
    {
        if (openInterface != null) { CloseInterface(); }

        openInterface = Instantiate(interfacePrefab, interfaceParent).GetComponent<StaticInterface>();
        openInterface.Init(entity, screenPosition + (Vector3.right * 20));

        return true;
    }

    public void CloseInterface()
    {
        Destroy(openInterface.gameObject);
        openInterface = null;
    } 

    public void ToggleOptionsMenu()
    {
        OptionsMenu.SetActive(!OptionsMenu.activeSelf);
    }

    public void ToggleBuildMenu()
    {
        ToggleMenu(ConstructionMenu);
        UpdateConstructionMenu();

        TutorialProxy.Action?.Invoke(TutorialEvent.BuildButtonPressed);
    }

    public void UpdateConstructionMenu()
    {
        ConstructionMenu.GetComponent<ConstructionMenuBuilder>().UpdateList();
        LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }

    public void ToggleScienceMenu()
    {
        ToggleMenu(TechTree);
        TutorialProxy.Action?.Invoke(TutorialEvent.TechTreeOpened);
    }

    public void MapButtonPressed() { ToggleMenu(MapMenu); }


    public void ToggleMenu(GameObject menu)
    {
        if (menu == null) { return; }

        var popout = menu.GetComponent<PopOutMenu>();

        if (popout != null) { popout.Toggle(); }

        var multipop = menu.GetComponent<MultiPopoutMenu>();

        if (multipop != null) { multipop.Toggle(); }
    }



    // Construction Tools //

    public void BulldozeButtonPressed()
    {
        GameManager.Instance.PlayerInputManager.ChangeInputState(PlayerInputManager.InputState.Demolish);
        AudioManager.Instance.PlaySound(AudioData.Instance.UI_BuildButton, 0.8f); 
    }

    public void ElectricalCoverageButtonPressed()
    {
        ElectricalCoverageRenderer.enabled = !ElectricalCoverageRenderer.enabled; 
        TutorialProxy.Action?.Invoke(TutorialEvent.PowerOverlayPressed);
        AudioManager.Instance.PlaySound(AudioData.Instance.UI_ElectricalOverlay, 0.8f);
    }

    public void HeatmapButtonPressed() { }


    // Viewport Swapping //

    public void ViewPortClicked()
    {
        //GameManager.Instance.CameraController.SwapViews();
    }

    public void LanderButtonPressed()
    {
        GameManager.Instance.CameraController.ResetPosition();
    }
}
