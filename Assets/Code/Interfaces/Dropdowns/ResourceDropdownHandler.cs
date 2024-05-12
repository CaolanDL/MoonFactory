using System; 
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResourceDropdownHandler : MonoBehaviour
{ 
    List<ResourceData> resources = new();
    List<GameObject> resourceIconElements = new();
    [SerializeField] ResourceIcon resourceIcon;
    [SerializeField] TMP_Text ActiveItemLabel;
    [SerializeField] Image ActiveItemImage;
    [SerializeField] GameObject gridLayout;

    public Action<ResourceData> callbackAction;  

    private void Awake()
    { 
        gridLayout.SetActive(false);
    }

    public void SetCallback(Action<ResourceData> callback)
    {
        callbackAction = callback; 
    }

    public void OnPressed()
    {
        gridLayout.SetActive(!gridLayout.activeSelf);
    }

    public void OptionSelected(ResourceData resource)
    { 
        SetSelected(resource); 
        callbackAction?.Invoke(resource);
        gridLayout.SetActive(false);
    } 

    public void SetSelected(ResourceData resource)
    {
        if(resource == null)
        {
            //resourceIcon.SetDetails(resource);
            ActiveItemImage.sprite = MenuData.Instance.emptySprite;
            ActiveItemLabel.text = "Select Option";
            return;
        }
        resourceIcon.SetDetails(resource);
        //ActiveItemLabel.text = resource.name;
        //ActiveItemImage.sprite = resource.sprite; 
    }

    public void Populate(List<ResourceData> _resources)
    {
        resources = _resources;

        foreach(var icon in resourceIconElements)
        {
            Destroy(icon);
        }
        resourceIconElements.Clear();

        foreach(ResourceData resource in resources)
        {
            var newIcon = Instantiate(MenuData.Instance.ResourceIcon_DropdownElement, gridLayout.transform);
            var dropdownItem = newIcon.GetComponent<ResourceDropDownItem>();
            dropdownItem.SetDetails(this, resource);
            resourceIconElements.Add(newIcon);
        } 
    }

    public void OptionSelected(TMP_Dropdown dropdown)
    {  
        if(dropdown.value == 0)
        {
            return;
        }
        callbackAction?.Invoke(resources[dropdown.value - 1]);  
    } 
}
