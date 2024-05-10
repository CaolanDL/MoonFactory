using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TutorialSequencer : MonoBehaviour
{
    [SerializeField] List<TutorialPopup> popupSequence;
    int activePopupIndex = 0;

    private void Awake()
    {
        TutorialProxy.BeginTutorial += BeginTutorial;

        foreach(TutorialPopup popup in popupSequence)
        {
            popup.gameObject.SetActive(false);
        }
    }

    public void BeginTutorial()
    {
        popupSequence[0].gameObject.SetActive(true); 
    }

    public void SkipTutorial()
    {
        Destroy(gameObject);
    }

    public void AdvanceTutorial()
    {
        popupSequence[activePopupIndex].gameObject.SetActive(false);
        activePopupIndex++;
        popupSequence[activePopupIndex].gameObject.SetActive(true);
    }
}
