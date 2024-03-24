using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine; 

public class RoverManager : MonoBehaviour
{
    public static List<Rover> Rovers = new();

    public GameObject roverDisplayObjectPrefab;

    public GameObject widgetDisplayObjectPrefab;

    private void Awake()
    {
        MakeSingleton();
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

        var newRoverDO = Instantiate(roverDisplayObjectPrefab, new Vector3(location.x, 0 , location.y), Quaternion.identity, transform).GetComponent<DisplayObject>();

        newRover.Init(location, newRoverDO);

        return newRover;
    }

    public Widget SpawnWidget(int2 location)
    {
        var widget = new Widget();

        Rovers.Add(widget);

        var widgetDO = Instantiate(widgetDisplayObjectPrefab, new Vector3(location.x, 0, location.y), Quaternion.identity, transform).GetComponent<DisplayObject>() ;

        widget.Init(location, widgetDO);

        return widget;
    }

    public static RoverManager Instance;
    public void MakeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
}