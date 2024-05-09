using System; 
using System.Collections.Generic;
using TMPro;
using UnityEngine; 

public class ResourceDropdownHandler : MonoBehaviour
{
    TMP_Dropdown dropdown;

    List<ResourceData> resources;

    List<TMP_Dropdown.OptionData> options = new();

    public Action<ResourceData> callbackAction;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    public void SetCallback(Action<ResourceData> callback)
    {
        callbackAction = callback; 
    }

    public void SetSelected(ResourceData resource)
    {
        if(resources != null)
        { 
            dropdown.value = resources.IndexOf(resource) + 1;
        }
        else
        {
            dropdown.value = -1;
        }
    }

    public void Populate(List<ResourceData> _resources)
    {
        resources = _resources;

        dropdown.value = -1;

        if (resources == null) { throw new Exception("No resouces given to resource dropdown"); }

        options.Clear();

        options.Add(new(""));

        foreach (ResourceData resource in resources)
            options.Add(new(resource.name, resource.sprite));

        

        dropdown.ClearOptions();
        dropdown.AddOptions(options); 
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
