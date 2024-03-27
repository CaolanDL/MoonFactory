using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Singletons/Menu Data")]
public class MenuData : ScriptableObject
{
    [Header("Main Menu")]
    public GameObject mainMenu;
    public GameObject MobilePlatformWarning;

    [Header("HUD")] 
    public GameObject HUD;

    [Header("Tool Tips")]
    public GameObject BuildButtonTooltip;

    [Header("Interfaces")]
    public GameObject CraftingMachineInterface;
    public GameObject HopperInterface;

    [Header("Interfaces Elements")]
    public GameObject InventoryElement;


    [Header("Generic Sprites")]
    public Sprite emptySprite;



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
