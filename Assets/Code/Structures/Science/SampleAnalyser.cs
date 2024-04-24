using RoverTasks;
using UnityEngine;

public class SampleAnalyser : Structure, IRecieveResources, IRequestResources
{
    ManagedTask RequestTask = new(); 
    ResourceData requestResource; 
    public ResourceData sample;
    public bool isResearching = false;
    public int progress = 0; 

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

        }
    }

    void BeginResearch()
    {
        isResearching = true;
    }

    void DoResearch()
    {
        progress += 1;
        if(progress > sample.TicksToResearch)
        {
            GameManager.Instance.ScienceManager.ResearchComplete(sample, ScienceManager.Researcher.Analyser);
        }
    }

    public void SetRequest(ResourceData resource, int quantity)
    {
        requestResource = resource;
        if (!isResearching) { NewRequest(); }
    } 

    public void NewRequest()
    {
        if (RequestTask.taskExists) { RequestTask.CancelTask(); }
        RequestTask.TryCreateTask(new RoverTasks.SoftRequestResourceTask(requestResource, 1));
    }

    public void RecieveResources(ResourceData resource, int quantity)
    { 
        sample = resource;
        BeginResearch();
    }


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
}