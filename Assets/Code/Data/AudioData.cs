using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Singletons/Audio Data")]
public class AudioData : ScriptableObject
{
    [Header("UI Sounds")] 
    public AudioClip UI_MenuOpen;
    public AudioClip UI_MenuClose;

    public AudioClip UI_InterfaceOpen;
    public AudioClip UI_InterfaceClose; 

    public AudioClip UI_BuildButton;
    public AudioClip UI_ScienceNodeUnlocked;

    public AudioClip UI_ElectricalOverlay;
    public AudioClip UI_Bulldoze;

    public AudioClip UI_PopupResearchComplete;
    public AudioClip UI_PopupUnlock;

    public AudioClip UI_Error;

    [Header("Tool Sounds")]
    public AudioClip Tool_PlaceGhost;
    public AudioClip Tool_DeleteGhost;
    public AudioClip Tool_Bulldoze;

    [Header("World Sounds")]
    public AudioClip World_StructureConstructed;
    public AudioClip World_StructureDemolished;

    [Header("Rover Sounds")]
    public AudioClip UI_WidgetInterfaceOpened;
    public AudioClip Rover_CollectResource;
    public AudioClip Rover_DeliverResource;
    public AudioClip Rover_Constructing;
    public AudioClip Rover_Idle;


    #region Singleton Instance
    public static AudioData Instance { get; private set; }
     
    private void OnEnable() { MakeSingleton(); } 
    private void OnValidate() { MakeSingleton(); }

    public void MakeSingleton()
    {
         Instance = this; 
    }
    #endregion
}
