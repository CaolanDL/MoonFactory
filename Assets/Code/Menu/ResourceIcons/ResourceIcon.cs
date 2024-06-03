using System;
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

    [NonSerialized] public ResourceData resource;
    [NonSerialized] public int count;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (resource == null) return;
        if (tooltip != null) return; 

        tooltip = Instantiate(ToolTipPrefab, GameManager.Instance.HUDManager.transform);
        tooltip.GetComponent<ResourceToolTip>().SetDetails(resource);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyTooltip();
    }

    private void OnDisable() => DestroyTooltip(); 
    private void OnDestroy() => DestroyTooltip(); 

    public void DestroyTooltip()
    {
        Destroy(tooltip);
    }

    public void SetDetails(ResourceData resouce)
    {
        this.resource = resouce;

        if(resource != null)
        {  
            SetSprite(resource.sprite);
            SetName(resource.name);
        }
        else
        {
            SetSprite(MenuData.Instance.emptySprite);
            SetName("None"); 
        } 
    }

    void SetSprite(Sprite sprite)
    {
        if(this.sprite != null)
        {
            this.sprite.sprite = sprite;
        }
    }

    void SetName(string name)
    {
        if(iconName != null)
        {
            iconName.text = name;
        }
    }

    public void SetCount(int count)
    {
        if(counter != null && this.count != count)
        {
            counter.text = count.ToString();
            this.count = count;
            gameObject.AddComponent<GraphicsPulseOnce>();
        } 
    }
}