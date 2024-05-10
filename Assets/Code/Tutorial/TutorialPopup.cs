using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] bool isSkippable = false;
    [SerializeField] LinkedEvent linkedEvent;
    public enum LinkedEvent
    {

    }

    [Header("References")]
    [SerializeField] GameObject skipText;
    TutorialSequencer tutorialSequencer;

    private void Awake()
    {
        tutorialSequencer = GetComponentInParent<TutorialSequencer>();

        if (!isSkippable && skipText != null)
        {
            Destroy(skipText);
        }
    }

    void BindEvent()
    {
        switch (linkedEvent)
        {
            case LinkedEvent.None:
                break;
            case LinkedEvent.BuildButtonPressed:
                TutorialProxy.BuildButtonPressed += AdvanceTutorial; break;
            case LinkedEvent.StaticDrillSelected:
                TutorialProxy.StaticDrillSelected += AdvanceTutorial; break;
            case LinkedEvent.StaticDrillInterfaceOpened:
                TutorialProxy.StaticDrillInterfaceOpened += AdvanceTutorial; break;
            case LinkedEvent.LanderInterfaceOpened:
                TutorialProxy.LanderInterfaceOpened += AdvanceTutorial; break;
            case LinkedEvent.TechTreeOpened:
                TutorialProxy.TechTreeOpened += AdvanceTutorial; break; 
        }
    }

    public void PopupClicked()
    {
        if (isSkippable)
        {
            AdvanceTutorial();
        }
    }

    public void AdvanceTutorial()
    {
        tutorialSequencer.AdvanceTutorial();
    }


}
