using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Singletons/Menu Data")]
public class MenuData : ScriptableObject
{
    [Header("Main Menu")]
    public GameObject SplashScreen;
    public GameObject mainMenu;
    public GameObject MobilePlatformWarning;

    [Header("HUD")] 
    public GameObject HUD;

    [Header("Tool Tips")]
    public GameObject BuildButtonTooltip;
    public GameObject GenericTooltipPrefab;

    [Header("Interfaces")]
    public GameObject ModularInterface;
    public GameObject RoverInterface;
    public GameObject CraftingMachineInterface;
    public GameObject HopperInterface;
    public GameObject RelayInterface;
    public GameObject SampleAnalyserInterface;
    public GameObject LanderInterface;
    public GameObject StaticDrillInterface;

    [Header("Interfaces Elements")]
    public GameObject InventoryElement;

    [Header("Resource Icons")]
    public GameObject ResourceIcon_DropdownElement;

    [Header("Animation Effects")]
    public Component PulseOnce;

    [Header("Generic Sprites")]
    public Sprite emptySprite;
    public GameObject DemolishSprite;


    public static MenuData Instance;

    private void Awake()
    {
        MakeSingleton();
    }

    private void OnEnable()
    {
        MakeSingleton(); 
    }

    private void OnValidate()
    {
        MakeSingleton(); 
    } 

    public void MakeSingleton()
    {
        if (Instance != null && Instance != this)  Destroy(this); 
        else Instance = this; 
    }
}
