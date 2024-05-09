using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image sprite;
    [SerializeField] TMP_Text iconName;
    [SerializeField] TMP_Text counter;

    GameObject tooltip;
    [SerializeField] GameObject ToolTipPrefab;

    public ResourceData resource;
    public int count;

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

    public void SetCount(int count)
    {
        if(counter != null)
        {
            counter.text = count.ToString();
            this.count = count;
        }
    }
}