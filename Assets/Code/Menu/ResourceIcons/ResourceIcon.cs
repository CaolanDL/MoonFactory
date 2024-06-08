using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResourceIcon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler//, IPointerClickHandler
{
    [SerializeField] Image sprite;
    [SerializeField] TMP_Text iconName;
    [SerializeField] TMP_Text counter;

    private Button parentButton;

    GameObject tooltip;
    [SerializeField] GameObject ToolTipPrefab;

    [NonSerialized] public ResourceData resource;
    [NonSerialized] public int count;

    bool isMouseOver = false;

    private void Awake()
    {
        parentButton = GetComponent<Button>();
        if (parentButton == null) { parentButton = gameObject.GetComponentInParent<Button>(); }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (resource == null) return;
        if (tooltip != null) return; 

        tooltip = Instantiate(ToolTipPrefab, GameManager.Instance.HUDManager.transform);
        tooltip.GetComponent<ResourceToolTip>().SetDetails(resource);

        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DestroyTooltip();

        isMouseOver = false;
    }

    private void Update()
    {
        if (isMouseOver && GameManager.Instance.PlayerInputManager.inputActions.UIControls.RightClick.WasPressedThisFrame())
        {
            HUDManager.OpenResourceInterface?.Invoke(resource);
        }
    }

/*    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            HUDManager.OpenResourceInterface?.Invoke(resource);
        }
        if (parentButton != null) { parentButton.OnPointerClick(eventData); } 
    }*/

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
            counter.text = "x"+count.ToString();
            this.count = count;
            DoPulse();
        } 
    }

    void DoPulse()
    {
        if(GetComponent<GraphicsPulseOnce>() != null) return;
        gameObject.AddComponent<GraphicsPulseOnce>();
    } 
}