using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialPopup : MonoBehaviour
{
    [Header("Options")]
    [SerializeField] bool isSkippable = false;
    [SerializeField] bool isMoveable = false;
    [SerializeField] TutorialEvent linkedEvent;
    [SerializeField] TutorialTag popupTag;

    [Header("References")]
    [SerializeField] TMP_Text skipText; 
    TutorialSequencer tutorialSequencer;

    private void Awake()
    {
        tutorialSequencer = GetComponentInParent<TutorialSequencer>();

        if (!isSkippable && skipText != null)
        {
            skipText.text = "<s>Continue</s>";
        }
    }

    private void OnEnable()
    {
        TutorialProxy.Action += GameEvent;
        TutorialProxy.SetPopupPosition += SetPosition;
    }

    private void OnDisable()
    {
        TutorialProxy.Action -= GameEvent;
        TutorialProxy.SetPopupPosition -= SetPosition;
    }

    void GameEvent(TutorialEvent @event)
    {
        if(@event != linkedEvent) { return; }

        AdvanceTutorial();
    }

    void SetPosition(Vector3 pos, TutorialTag tag)
    {
        if(tag != popupTag) return;
        if(!isMoveable) return;
        transform.position = pos;
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
