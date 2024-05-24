using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    [SerializeField] Transform container;

    [Header("Popup Prefabs")]
    [SerializeField] GameObject ResourceUnlocked;
    [SerializeField] GameObject StructureUnlocked;
    [SerializeField] GameObject ResearchComplete;

    private void OnEnable() => SubscribeEvents(); 
    private void OnDisable() => UnsubscribeEvents(); 

    void SubscribeEvents()
    {
        ScienceManager.OnResourceUnlocked += Popup_ResourceUnlocked;
        ScienceManager.OnStructureUnlocked += Popup_StructureUnlocked;
        ScienceManager.ResearchCompleted += Popup_ResearchComplete;
    }
    void UnsubscribeEvents()
    {
        ScienceManager.OnResourceUnlocked -= Popup_ResourceUnlocked;
        ScienceManager.OnStructureUnlocked -= Popup_StructureUnlocked;
        ScienceManager.ResearchCompleted -= Popup_ResearchComplete;
    }

    public void Popup_ResourceUnlocked(ResourceData resource)
    {
        CreatePopup(ResourceUnlocked, resource.name, resource.sprite);
    }

    public void Popup_StructureUnlocked(StructureData structureData)
    {
        CreatePopup(StructureUnlocked, structureData.screenname, structureData.sprite);
    }

    public void Popup_ResearchComplete(ResourceData resource, int points)
    {
        var popup = CreatePopup(ResearchComplete, resource.name, resource.sprite);
        ((ResearchPopupHandler)popup).SetSciencePoints(points);
    }

    public PopupHandler CreatePopup(GameObject PopupPrefab, string name, Sprite sprite)
    {
        var popup = CreatePopup(PopupPrefab);
        popup.SetDetails(name, sprite);
        return popup;
    }
    public PopupHandler CreatePopup(GameObject PopupPrefab)
    {
        var newPopup = Instantiate(PopupPrefab, container);
        var popupHandler = newPopup.GetComponent<PopupHandler>();
        popupHandler.Init(this);
        return popupHandler;
    }
}
