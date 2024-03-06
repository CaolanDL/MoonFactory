﻿using System;
using Unity.Mathematics;
using UnityEngine;

public static class DevFlags
{
#if UNITY_EDITOR

    public static bool SkipMainMenu = true;

#else

    public static bool SkipMainMenu = false;

#endif
}

public class GameManager : MonoBehaviour
{
    [Header("Global Data Blocks")]
    [SerializeField] public GlobalData globalData;
    [SerializeField] public WorldGenerationData worldGenerationData; 

    [Header("UI Prefabs")]
    public GameObject mainMenu;
    public GameObject HUD;
    public GameObject MobilePlatformWarning;

    //

    public GameWorld gameWorld;

    private FloorTileRenderer floorTileRenderer; 

    public ConstructionManager ConstructionManager;

    private void Awake()
    {
        MakeSingleton();

        globalData.MakeSingleton();
        worldGenerationData.MakeSingleton();

        floorTileRenderer = GetComponent<FloorTileRenderer>();
    }

    private void Start()
    {
        if (DevFlags.SkipMainMenu) { CreateNewGame("DevGame"); return; }

        if (Application.isMobilePlatform) { Instantiate(MobilePlatformWarning); return; }

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
        if(gameWorld == null) { return; }
        //Read Player inputs

        // Update Conveyors
        // Update electrical systems
        // Update rovers
        // Update Machines

        // Draw Ghosts
        ConstructionManager.DrawGhosts();

        // Draw Floor tiles
        floorTileRenderer.Tick();

        // Draw Items
    }



    public void CreateNewGame(string saveName)
    {
        // User input save file name

        // Random seed is chosen
        int seed = UnityEngine.Random.Range(0, int.MaxValue);

        // Create new gameWorld
        gameWorld = new GameWorld(seed);
        ConstructionManager = new();

        // Start zone is generated
        GenerateStartZone();
        gameWorld.DebugLogLocations();

        // Descent vehicle animation plays

        // UI startup animation plays
        Instantiate(HUD, transform);

        // Tutorial toggle prompt

        // Save game to file

        // Enable player input

    }

    public void OpenMainMenu()
    {
        Instantiate(mainMenu, transform);
    }

    void ExitToMenu()
    {
        // Save game to file
        // Load main menu scene
    }

    void LoadSave()
    {
        // user input save file name or selects from menu
        // begin deserialization of data
    }

    void SaveGame()
    {
        // Wait until end of update loop
        // Display save icon
        // Serialize Game Data
        // Remove saving icon
    }



    void GenerateStartZone()
    {
        int size = WorldGenerationData.StartZoneSize;

        int halfSize = size / 2; 
 
        for (int xChunk = -halfSize; xChunk < halfSize+1; xChunk++)
        {
            for (int yChunk = -halfSize; yChunk < halfSize+1; yChunk++)
            {
                gameWorld.GenerateChunk(new int2(xChunk, yChunk));
            }
        }
    }
}