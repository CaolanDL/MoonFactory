using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class ResearchDropdownManager : MonoBehaviour
{
    ResourceDropdownHandler dropdownHandler; 
    [SerializeField] ScienceManager.Researcher researcher;
    public IRequestResources researchRequestor;

    public Action<ResourceData> SetRequestResourceAction;

    private void Awake()
    {
        var researchInterface = GetComponentInParent<IResearchInterface>();
        researchRequestor = researchInterface.GetIRequestResources(); 
        dropdownHandler = GetComponent<ResourceDropdownHandler>();
        dropdownHandler.SetCallback(SetRequestResourceAction);
        SetRequestResourceAction += SetRequestResource;
    }

    private void Start()
    {
        List<ResourceData> resourceDatas = new();

        foreach(var entry in GameManager.Instance.ScienceManager.ResearchRegistries[researcher])
        {
            if(entry.Value == false) { resourceDatas.Add(entry.Key); }
        }

        dropdownHandler.Populate(resourceDatas);
    } 

    void SetRequestResource(ResourceData resource)
    {
        researchRequestor.SetRequest(resource);
    }
}
