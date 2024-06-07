using UnityEngine;
using UnityEngine.EventSystems;

public class GenericTooltipCreator : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] public string Header;
    [SerializeField] public string Content;
    [SerializeField] Vector2 Alignment;

    GameObject tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        CreateTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    { 
        DestroyTooltip();
    }

    private void OnDisable()
    {
        DestroyTooltip();
    }

    private void OnDestroy()
    {
        DestroyTooltip();
    }

    void CreateTooltip()
    {
        tooltip = Instantiate(MenuData.Instance.GenericTooltipPrefab, GameManager.Instance.HUDManager.transform);
        var handler = tooltip.GetComponent<GenericTooltipHandler>();
        handler.SetContent(Content);
        handler.SetHeader(Header);
        handler.SetAlignment(Alignment);
    }

    void DestroyTooltip()
    { 
        Destroy(tooltip); 
    }
}
