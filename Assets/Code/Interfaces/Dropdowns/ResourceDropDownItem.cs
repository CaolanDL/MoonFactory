using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDropDownItem : MonoBehaviour
{
    [SerializeField] ResourceIcon ResourceIcon;
    ResourceDropdownHandler parentHandler;

    ResourceData resource;

    private void Awake()
    {
        ResourceIcon = GetComponent<ResourceIcon>();
    }

    private void OnEnable()
    {
        ResourceIcon = GetComponent<ResourceIcon>();
    }

    public void SetDetails(ResourceDropdownHandler parent, ResourceData resource)
    {
        parentHandler = parent;
        this.resource = resource;
        ResourceIcon.SetDetails(resource);
    }

    public void OnClicked()
    {
        ResourceIcon.DestroyTooltip();
        parentHandler.OptionSelected(resource);
    }
}
