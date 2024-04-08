using System; 
using System.Collections.Generic;
using TMPro;
using UnityEngine; 

public class ResourceDropdownHandler : MonoBehaviour
{
    TMP_Dropdown dropdown;

    List<ResourceData> resources;

    List<TMP_Dropdown.OptionData> options = new();

    Action<ResourceData> callbackAction;

    private void Awake()
    {
        dropdown = GetComponent<TMP_Dropdown>();
    }

    public void SetCallback(Action<ResourceData> callback)
    {
        callbackAction = callback;
    }

    public void Populate(List<ResourceData> _resources)
    {
        resources = _resources;

        if (resources == null) { throw new Exception("No resouces given to resource dropdown"); }

        options.Clear();

        foreach (ResourceData resource in resources)
            options.Add(new(resource.name, resource.sprite));

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

    public void OptionSelected(TMP_Dropdown dropdown)
    {
        callbackAction.Invoke(resources[dropdown.value]);
    }
}
