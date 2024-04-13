using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine; 

public class RoverManager
{
    public static List<Rover> Rovers = new();

    public static Dictionary<Rover, int2> RoverPositions = new();

    public RoverData RoverData; 

    public RoverManager()
    {
        MakeSingleton();
        RoverData = GameManager.Instance.RoverData;
    }

    public static void TickRovers()
    {
        foreach (var rover in Rovers)
        {
            rover.Tick();
        }
    }

    public Rover SpawnNewRover(int2 location)
    {
        var newRover = new Rover();

        Rovers.Add(newRover);
        RoverPositions.Add(newRover, newRover.GridPosition);

        var newRoverDO = Object.Instantiate(RoverData.RoverDisplayObject, new Vector3(location.x, 0 , location.y), Quaternion.identity, GameManager.Instance.transform).GetComponent<DisplayObject>();

        newRover.SetDisplayObject(newRoverDO);
        newRover.SetPosition(location);

        return newRover;
    }

    public Widget SpawnWidget(int2 location)
    {
        var widget = new Widget();

        Rovers.Add(widget);

        var widgetDO = Object.Instantiate(RoverData.WidgetDisplayObject, new Vector3(location.x, 0, location.y), Quaternion.identity, GameManager.Instance.transform).GetComponent<DisplayObject>() ;

        widget.SetDisplayObject(widgetDO);
        widget.SetPosition(location);

        return widget;
    }

    public static RoverManager Instance;
    public void MakeSingleton()
    { 
        Instance = this; 
    }
}