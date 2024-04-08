﻿using Logistics;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;

public static class DevFlags
{
    public static bool SkipMainMenu = true;
}

//! MoonFactory
// This GameManager is effectively Main running as a monobehavior.

// Count LOC at the code directory with PowerShell: dir -Recurse *.cs | Get-Content | Measure-Object -Line
 
public class GameManager : MonoBehaviour
{
    [Header("Global Data")]
    [SerializeField] public GlobalData GlobalData;
    [SerializeField] public RenderData RenderData;
    [SerializeField] public TerrainGenerationData WorldGenerationData;
    [SerializeField] public MenuData MenuData;
    [SerializeField] public RoverData RoverData;

    // Gameplay Objects
    public GameWorld GameWorld;
    public ConstructionManager ConstructionManager;
    public RoverManager RoverManager;
    public TaskManager TaskManager;

    // Menu Instances
    public HUDController HUDController;

    // Components
    public FloorTileRenderer FloorTileRenderer;
    public ItemRenderer ItemRenderer;
    public BatchRenderer BatchRenderer;

    public CameraController CameraController;
    public PlayerInputManager PlayerInputManager;

    // Global Events
    public static Action OnGameExit;
    public static Action OnSaveLoad;
    public static Action OnStartNewGame;

    private void Awake()
    {
        Profiler.maxUsedMemory = 50000000;
        //Shader.WarmupAllShaders();
        //ShaderVariantCollection.WarmUp();

        MakeSingleton(); 

        GlobalData.MakeSingleton();
        WorldGenerationData.MakeSingleton();
        MenuData.MakeSingleton(); 

        RoverManager = GetComponent<RoverManager>();

        CameraController = GetComponent<CameraController>();
        PlayerInputManager = GetComponent<PlayerInputManager>();

        FloorTileRenderer = GetComponent<FloorTileRenderer>();
        FloorTileRenderer.Init();

        ItemRenderer = GetComponent<ItemRenderer>();
        ItemRenderer.Init();

        BatchRenderer = GetComponent<BatchRenderer>();
        BatchRenderer.Init();
    }

    private void Start()
    {
#if UNITY_EDITOR
        if (DevFlags.SkipMainMenu) { CreateNewGame("DevGame"); DebugUnlockAll(); return; }
#endif

        if (Application.isMobilePlatform) { Instantiate(MenuData.MobilePlatformWarning); return; }
          
        OpenMainMenu();
    }

    #region Singleton Instanciation
    public static GameManager Instance { get; private set; }

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
    #endregion

    private void Update()
    {
        if(GameWorld == null) { return; }
        //Read Player inputs
         
        // Draw Ghosts
        ConstructionManager.DrawGhosts();

        // Do Batch Rendering
        FloorTileRenderer.Tick(); 
        ItemRenderer.Tick();
        BatchRenderer.Tick();

        // Call Frame Update on Structures
        Structure.FrameUpdateAllStructures();
    }

    private void FixedUpdate()
    {
        if (GameWorld == null) { return; }

        GameWorld.OnFixedUpdate();

        // Update Conveyors 
        ChainManager.UpdateChains();
        // Update rovers
        RoverManager.TickRovers();
        // Update Machines
        Structure.TickAllStructures(); 
    }

    public void CreateNewGame(string saveName)
    {
        // User input save file name

        // Random seed is chosen
        int seed = UnityEngine.Random.Range(0, int.MaxValue);

        // Create new gameWorld
        GameWorld = new GameWorld(seed);
        ConstructionManager = new(); 
        TaskManager = new TaskManager();

        // Start zone is generated
        GameWorld.GenerateStartZone(); 

        // Descent vehicle animation plays

        // UI startup animation plays
        HUDController = Instantiate(MenuData.HUD, transform).GetComponent<HUDController>();

        // Tutorial toggle prompt

        // Save game to file

        // Enable player input
         
    }

    public void OpenMainMenu()
    {
        Instantiate(MenuData.mainMenu, transform);
    }

    void ExitToMenu()
    {
        // Save game to file
        // Unload gameworld
        // Destroy HUD
        // Load main menu scene
    }

    void LoadSave()
    {
        // user input save file name or selects from menu
        // begin deserialization of data
        // Load an empty gameWorld
        // Populate with entities from save file
        // Load HUD
        // Enable player input
    }

    void SaveGame()
    {
        // Wait until end of update loop
        // Display save icon
        // Serialize Game Data
        // Remove saving icon
    }

    // DEVELOPMENT // 
      
    public void DebugBuildBenchmark()
    {
        int width = 126; 
        int offset = 12;

        StructureData debugOutput = GetStructureData("DebugOutput");

        StructureData conveyor = GetStructureData("Conveyor");

        StructureData crusher = GetStructureData("Crusher");

        StructureData magSep = GetStructureData("MagneticSeperator");

        StructureData hopper = GetStructureData("Hopper");

        StructureData GetStructureData(string name) { return GlobalData.Structures.Find(structure => structure.name == name); } 

        for (int x = 0; x < width; x += 2)
        {
            var y = offset;

            ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, debugOutput); y++;

            ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, conveyor); y++;

            ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, hopper); y++;

            /* ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, crusher); y++;

             ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, conveyor); y++;

             ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, magSep); y++;

             ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, conveyor); y++;
             ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, conveyor); y++;*/
        }
    }

    public void DebugSpawnRover()
    {
        RoverManager.SpawnNewRover(new int2(0,0));
    } 

    public void DebugUnlockAll()
    {
        foreach(StructureData i in GlobalData.Structures)
        {
            i.Unlock();
        }
        foreach (ResourceData i in GlobalData.Resources)
        {
            i.Unlock();
        }
        foreach (CraftingFormula i in GlobalData.CraftingFormulas)
        {
            i.Unlock();
        }
    }
}