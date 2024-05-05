using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image sprite;
    [SerializeField] TMP_Text iconName;

    GameObject tooltip;
    [SerializeField] GameObject ToolTipPrefab;

    ResourceData resource;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltip != null) return;

        tooltip = Instantiate(ToolTipPrefab, GameManager.Instance.HUDManager.transform);
        tooltip.GetComponent<ResourceToolTip>().SetDetails(resource);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Destroy(tooltip);
    }

    public void SetDetails(ResourceData resouce)
    {
        this.resource = resouce;

        sprite.sprite = resource.sprite; 
        iconName.text = resource.name;   
    }
}