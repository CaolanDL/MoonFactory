using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuildStructureButton : MonoBehaviour
{   
    [NonSerialized] public StructureData structureData;

    public Image image;

    public TextMeshProUGUI nameText;

    private UIMouseHover mouseHover;

    public void Awake()
    {
        mouseHover = GetComponent<UIMouseHover>();
    }

    public void Start()
    {
        mouseHover = GetComponent<UIMouseHover>();
    }

    public void ConfigureButton(StructureData structureData)
    {
        this.structureData = structureData;

        image = GetComponent<Image>();

        nameText = GetComponentInChildren<TextMeshProUGUI>();

        if (structureData.sprite != null)
        {
            image.sprite = structureData.sprite;
        }
         
        nameText.SetText(structureData.screenname); 
    } 

    public void ButtonPressed()
    {
        var gameManager = GameManager.Instance;

        gameManager.GetComponent<PlayerInputManager>().ChangeInputState(PlayerInputManager.InputState.Default);

        gameManager.ConstructionManager.StartPlacingGhosts(structureData);

        gameManager.GetComponent<PlayerInputManager>().ChangeInputState(PlayerInputManager.InputState.Construction);

        HandlePressedTutorialEvents();
    }

    void HandlePressedTutorialEvents()
    {
        if (structureData.name == "StaticDrill")
        {
            TutorialProxy.Action?.Invoke(TutorialEvent.StaticDrillSelected);
        }
        if (structureData.name == "SampleAnalyser")
        {
            TutorialProxy.Action?.Invoke(TutorialEvent.SampleAnalyserSelected);
        }
    }


    static float toolTipCountTime = 0.2f;
    float toolTipCountDown = toolTipCountTime;
    GameObject activeToolTip = null;

    private void Update()
    {
        if (mouseHover.IsMouseOver)
        {
            if (toolTipCountDown > 0) { toolTipCountDown -= Time.deltaTime; }

            else if (activeToolTip == null)
            {
                activeToolTip = Instantiate(MenuData.Instance.BuildButtonTooltip, GameManager.Instance.HUDManager.transform);
                activeToolTip.GetComponent<StructureTooltip>().SetDetails(structureData);

            }
            if (activeToolTip != null)
            {
                //activeToolTip.transform.position = new Vector3(mouseHover.pointerPosition.x, 0, mouseHover.pointerPosition.y);
            }
        }
        else if(toolTipCountDown > 0)
        {
            toolTipCountDown = Mathf.Clamp(toolTipCountDown += Time.deltaTime, 0, toolTipCountTime); 
        }
        else if(toolTipCountDown <= 0)
        {
            if (activeToolTip != null) { Destroy(activeToolTip); }
        }

        HandleUpdateTutorialEvents();
    } 

    void HandleUpdateTutorialEvents()
    {
        if(structureData.name == "StaticDrill")
        {
            TutorialProxy.SetPopupPosition?.Invoke(transform.position, TutorialTag.SelectStaticDrill);
        } 
    }
}
