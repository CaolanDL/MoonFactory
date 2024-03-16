using Logistics;
using Unity.Mathematics;
using UnityEngine;

public static class DevFlags
{
#if UNITY_EDITOR

    public static bool SkipMainMenu = true;
    public static bool Benchmark = false;

#else

    public static bool SkipMainMenu = false;

#endif
}

public delegate void OnGameExit(); 

public delegate void OnSaveLoad();

public delegate void OnStartNewGame(); 


public class GameManager : MonoBehaviour
{
    [Header("Global Data Blocks")]
    [SerializeField] public GlobalData GlobalData;
    [SerializeField] public WorldGenerationData worldGenerationData; 

    [Header("UI Prefabs")]
    public GameObject mainMenu;
    public GameObject HUD;
    public GameObject MobilePlatformWarning;

    //

    public GameWorld gameWorld;

    public FloorTileRenderer floorTileRenderer;
    public ItemRenderer itemRenderer;

    public ConstructionManager ConstructionManager;

    public CameraController cameraController;

    private void Awake()
    {
        MakeSingleton();

        cameraController = GetComponent<CameraController>();

        GlobalData.MakeSingleton();
        worldGenerationData.MakeSingleton();

        floorTileRenderer = GetComponent<FloorTileRenderer>();
        floorTileRenderer.Init();

        itemRenderer = GetComponent<ItemRenderer>();
        itemRenderer.Init();
    }

    private void Start()
    {
        if(DevFlags.Benchmark == true)
        {
            CreateNewGame("DevGame"); BuildCPUBenchmark(128, 64) ; return;
        }

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
        gameWorld.OnFixedUpdate();

        // Update Conveyors 
        ChainManager.UpdateChains();
        // Update rovers
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

        // Start zone is generated
        gameWorld.GenerateStartZone(); 

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

    void BuildCPUBenchmark(int width, int length)
    {
        StructureData debugOutput = GetStructureData("DebugOutput");

        StructureData conveyor = GetStructureData("Conveyor");

        StructureData GetStructureData(string name) { return GlobalData.structures.Find(structure => structure.name == name); }


        for (int x = 0; x < width; x++)
        {
            ConstructionManager.ForceSpawnStructure(new int2(x, 0), 0, debugOutput);

            for(int y = 1; y < length; y++)
            {
                ConstructionManager.ForceSpawnStructure(new int2(x, y), 0, conveyor);
            }
        }
    }
}