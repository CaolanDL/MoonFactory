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

    public static void FrameUpdateRovers()
    {
        foreach (var rover in Rovers)
        {
            rover.OnFrameUpdate();
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

    public void SpawnWidgetDropship()
    {
        var worldGrid = GameManager.Instance.GameWorld.worldGrid;
        int2 spawnLocation = new(0,0);
        int range = 1;
        while (SearchRange(range) == false) { range++; } 
        bool SearchRange(int radius)
        {
            for (int x = -radius; x < radius; x++)
            {
                for (int y = -radius; y < radius; y++)
                {
                    if (worldGrid.IsEntityAt(new(x, y))) { continue; }
                    spawnLocation = new(x, y);
                    return true;
                }
            }
            return false;
        }

        var dropship = Object.Instantiate(RoverData.WidgetDropShip);
        dropship.transform.position = new Vector3(spawnLocation.x, 0 , spawnLocation.y);
        dropship.GetComponentInChildren<DropShipSequence>().spawnPoint = spawnLocation;
    }

    public Widget SpawnNewWidget(int2 location)
    {
        var widget = new Widget();

        Rovers.Add(widget);
        RoverPositions.Add(widget, widget.GridPosition);

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