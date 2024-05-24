using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class ResearchDropdownManager : MonoBehaviour
{
    ResourceDropdownHandler dropdownHandler;
    IResearchInterface researchInterface;
    [SerializeField] ScienceManager.Researcher researcher;
    public IRequestResources researchRequestor;
    ResourceData requestResource;

    bool isStarting = true;

    public Action<ResourceData> SetRequestResourceAction; 

    private void Start()
    {
        researchInterface = GetComponentInParent<IResearchInterface>();
        researchRequestor = researchInterface.GetIRequestResources();
        dropdownHandler = GetComponent<ResourceDropdownHandler>();

        SetRequestResourceAction += ChangeResource;
        researchInterface.researcher.ResearchComplete += SetOptions;
        dropdownHandler.SetCallback(SetRequestResourceAction);

        SetOptions();

        dropdownHandler.SetSelected(researchRequestor.GetRequest());

        isStarting = false;
    }

    private void OnDestroy()
    {
        researchInterface.researcher.ResearchComplete -= SetOptions;
    }

    void SetOptions()
    {
        List<ResourceData> resourceDatas = new();

        foreach (var entry in GameManager.Instance.ScienceManager.ResearchRegistries[researcher])
        {
            if (GameManager.Instance.ScienceManager.unlocked_resources.Contains(entry.Key) && entry.Value == false) { resourceDatas.Add(entry.Key); }
        }

        dropdownHandler.Populate(resourceDatas);
    }

    void ChangeResource(ResourceData resource)
    { 
        if(isStarting) { return; }
        requestResource = resource;
        researchRequestor.SetRequest(requestResource);
    } 
}
