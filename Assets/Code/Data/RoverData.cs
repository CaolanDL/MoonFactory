using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Rover Data")]
public class RoverData : ScriptableObject   
{
    [Header("Display")]
    public string DefaultScreenName = "Rover: None";
    public string MiningScreenName = "Rover: Mining";
    public string LogisticsScreenName = "Rover: Logistics";
    public string ConstructionScreenName = "Rover: Construction";

    [Multiline]
    public string Description = "";

    [Header("Sprites")]
    public Sprite DefaultSprite;
    public Sprite MiningSprite;
    public Sprite LogisticsSprite;
    public Sprite ConstructionSprite;

    [Header("Prefabs")] 
    public GameObject RoverDisplayObject; 
    public GameObject WidgetDisplayObject;
}
