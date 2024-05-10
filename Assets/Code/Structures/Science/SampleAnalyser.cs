using RoverTasks;
using System;
using UnityEngine;

public class SampleAnalyser : Structure, IRecieveResources, IRequestResources, IDoResearch
{
    ManagedTask RequestTask = new(); 
    public ResourceData requestResource; 
    public ResourceData sample;
    public bool isResearching = false;
    public float progress = 0;

    public Action ResearchComplete { get; set; }

    public override void OnClicked(Vector3 mousePosition)
    {
        GameManager.Instance.HUDManager.OpenInterface(MenuData.Instance.SampleAnalyserInterface, this, mousePosition);
    }

    public override void OnConstructed()
    {
        base.OnConstructed();
    }

    public override void OnTick()
    {
        base.OnTick();

        if (isResearching)
        {
            DoResearch();
        }
    } 

    void DoResearch()
    {
        progress += 1 / (sample.SecondsToResearch * 50f); 
        if (progress >= 1)
        {
            FinishResearch();
        }
    }

    public void SetRequest(ResourceData resource, int quantity)
    {
        requestResource = resource;
        if (!isResearching) { NewRequest(); }
    } 

    public ResourceData GetRequest()
    {
        return requestResource;
    }

    public void NewRequest()
    {
        if (RequestTask.taskExists) { RequestTask.CancelTask(); }
        RequestTask.TryCreateTask(new RoverTasks.SoftRequestResourceTask(requestResource, 1, position));
    }

    public void RecieveResources(ResourceData resource, int quantity)
    {
        Debug.Log("sample recieved");
        sample = resource;
        BeginResearch();
    }

    void BeginResearch()
    {
        isResearching = true;
        DisplayObject.SetLoopingAnimation("Analysing");
    }

    void FinishResearch()
    {
        GameManager.Instance.ScienceManager.ResearchComplete(sample, ScienceManager.Researcher.Analyser);
        progress = 0;
        isResearching = false;
        sample = null;
        requestResource = null;

        ResearchComplete?.Invoke();

        DisplayObject.SetLoopingAnimation("Idle");
    }
}

public interface IDoResearch
{
    public Action ResearchComplete { get; set; }    
}

public interface IRecieveResources
{
    public void RecieveResources(ResourceData resource, int quantity);
}

public interface IRequestResources
{
    public void SetRequest(ResourceData resource)
    {
        SetRequest(resource, 1);
    }
    public void SetRequest(ResourceData resource, int quantity); 

    public ResourceData GetRequest()
    {
        return null;
    }
}