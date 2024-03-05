using System;
using Unity.Mathematics;
using UnityEngine;

public static class DevFlags
{
    public static bool SkipMainMenu = true;
}

public class GameManager : MonoBehaviour
{ 
    [SerializeField] public WorldGenerationData worldGenerationData;
    [SerializeField] public GlobalData globalData;

    public GameWorld gameWorld; 

    public GameObject mainMenu;
    public GameObject HUD;


    private FloorTileRenderer floorTileRenderer;
     
    [SerializeField] public ConstructionManager ConstructionManager;


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

        Instantiate(mainMenu, transform);
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