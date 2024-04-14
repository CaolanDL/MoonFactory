 using Unity.Mathematics;
using UnityEngine;
using RoverTasks;

public class SampleAnalyser : Structure
{
    ManagedTask RequestTask { get; set; }

    public override void OnClicked(Vector3 mousePosition)
    {
        GameManager.Instance.HUDManager.OpenInterface(MenuData.Instance.SampleAnalyserInterface, this, mousePosition);
    }
}

public class RocketPad : Structure
{ 
    public RocketPad()
    {
        base.size = new(3, 3);
    }
}

// Placeholder: Please Delete
public class ScienceRocket : Structure
{

} 

public class Railgun : Structure
{

}