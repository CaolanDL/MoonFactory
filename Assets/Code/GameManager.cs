using Logistics;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;

public static class DevFlags
{
#if UNITY_EDITOR

    public static bool SkipMainMenu = true;
    public static bool Benchmark = true; 
    public static bool AutoSpawnRover = true;
    public static bool RoverTaskOverrideToPathfind = true;

#else

    public static bool SkipMainMenu = false; 

#endif
}  

public class GameManager : MonoBehaviour
{
    [Header("Global Data Blocks")]
    [SerializeField] public GlobalData GlobalData;
    [SerializeField] public TerrainGenerationData worldGenerationData;
    [SerializeField] public MenuData menuData;

    // Gameplay Objects
    public GameWorld gameWorld;
    public ConstructionManager ConstructionManager;
    public RoverManager RoverManager;
    public TaskManager TaskManager;

    // Menu Instances
    public HUDController HUDController;

    // Manager Components
    public FloorTileRenderer floorTileRenderer;
    public ItemRenderer itemRenderer; 
    public CameraController cameraController;
    public PlayerInputManager playerInputManager;

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
        worldGenerationData.MakeSingleton();
        menuData.MakeSingleton(); 

        RoverManager = GetComponent<RoverManager>();

        cameraController = GetComponent<CameraController>();
        playerInputManager = GetComponent<PlayerInputManager>();

        floorTileRenderer = GetComponent<FloorTileRenderer>();
        floorTileRenderer.Init();

        itemRenderer = GetComponent<ItemRenderer>();
        itemRenderer.Init();
    }

    private void Start()
    {
        #if UNITY_EDITOR
        if(DevFlags.Benchmark == true) { CreateNewGame("DevGame"); BuildCPUBenchmark(64) ; return; }

        if (DevFlags.AutoSpawnRover) { CreateNewGame("DevGame"); AutoSpawnRover(); return; }

        if (DevFlags.SkipMainMenu) { CreateNewGame("DevGame"); return; }
#endif

        if (Application.isMobilePlatform) { Instantiate(menuData.MobilePlatformWarning); return; }
          
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
         
        // Draw Ghosts
        ConstructionManager.DrawGhosts();

        // Draw Floor tiles
        floorTileRenderer.Tick();

        // Draw Items
        itemRenderer.Tick();

        // Call Frame Update on Structures
        Structure.FrameUpdateAllStructures();
    }

    private void FixedUpdate()
    {
        if (gameWorld == null) { return; }

        gameWorld.OnFixedUpdate();

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
        gameWorld = new GameWorld(seed);
        ConstructionManager = new(); 
        TaskManager = new TaskManager();

        // Start zone is generated
        gameWorld.GenerateStartZone(); 

        // Descent vehicle animation plays

        // UI startup animation plays
        HUDController = Instantiate(menuData.HUD, transform).GetComponent<HUDController>();

        // Tutorial toggle prompt

        // Save game to file

        // Enable player input

    }

    public void OpenMainMenu()
    {
        Instantiate(menuData.mainMenu, transform);
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

#if UNITY_EDITOR

    void BuildCPUBenchmark(int width)
    {
        StructureData debugOutput = GetStructureData("DebugOutput");

        StructureData conveyor = GetStructureData("Conveyor");

        StructureData crusher = GetStructureData("Crusher");

        StructureData magSep = GetStructureData("MagneticSeperator");


        StructureData GetStructureData(string name) { return GlobalData.structures.Find(structure => structure.name == name); } 

        for (int x = 0; x < width; x += 2)
        {
            var y = 0;
            ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, debugOutput); y++;

            ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, conveyor); y++;

            ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, crusher); y++;

            ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, conveyor); y++;

            ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, magSep); y++;

            ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, conveyor); y++;
            ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, conveyor); y++;
        }
    }

    void AutoSpawnRover()
    {
        RoverManager.SpawnNewRover(new int2(0,0));
    }

#endif
}