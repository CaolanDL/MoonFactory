using Logistics;
using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Profiling;
using UnityEngine.UIElements;

public static class DevFlags
{
#if UNITY_EDITOR
    public static bool SkipMainMenu = true;
#else
    public static bool SkipMainMenu = false;
#endif
    public static bool InstantBuilding = false;
}

//! MoonFactory

// Count LOC at the code directory with PowerShell: dir -Recurse *.cs | Get-Content | Measure-Object -Line
 
public class GameManager : MonoBehaviour
{
    [Header("Global Data")]
    [SerializeField] public GlobalData GlobalData;
    [SerializeField] public RenderData RenderData;
    [SerializeField] public TerrainGenerationData WorldGenerationData;
    [SerializeField] public MenuData MenuData;
    [SerializeField] public RoverData RoverData;
    [SerializeField] public AudioData AudioData;

    // Manager Components
    public GameWorld GameWorld;
    public ScienceManager ScienceManager;
    public ConstructionManager ConstructionManager;
    public RoverManager RoverManager;
    public TaskManager TaskManager;
    public Electrical.SystemManager ElectricalSystemManager;
    public AssetWarmer AssetWarmer; 

    // Menu Instances
    public HUDManager HUDManager;
    public TutorialSequencer TutorialSequencer;

    // GameObjects
    public GameObject GameLighting;

    //Runtime References
    public Lander Lander;
    public AudioSource AudioSource;

    // Monobehavior Components
    public FloorTileRenderer FloorTileRenderer;
    public ItemRenderer ItemRenderer;
    public BatchRenderer BatchRenderer;

    public CameraController3D CameraController;
    public PlayerInputManager PlayerInputManager;

    public AudioManager AudioManager;

    // Main Menu
    public GameObject MainMenuCameraPrefab;
    [NonSerialized] public Camera _mainMenuCamera;
    public GameObject MainMenu;
    public GameObject SplashScreen;

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
        AudioData.MakeSingleton();

        CameraController = GetComponent<CameraController3D>();
        PlayerInputManager = GetComponent<PlayerInputManager>();

        FloorTileRenderer = GetComponent<FloorTileRenderer>();
        FloorTileRenderer.Init();

        ItemRenderer = GetComponent<ItemRenderer>();
        ItemRenderer.Init();

        BatchRenderer = GetComponent<BatchRenderer>();
        BatchRenderer.Init();

        AudioManager = GetComponent<AudioManager>();
        AudioManager.Instance = AudioManager;

        AssetWarmer = new();
        AssetWarmer.Warmup();
        AssetWarmer = null;
    }

    private void Start()
    {
        if (DevFlags.SkipMainMenu) { CreateNewGame(""); return; }
        if (Application.isMobilePlatform) { Instantiate(MenuData.MobilePlatformWarning); return; }

        PlaySplashScreen();
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
        
        GameWorld.OnUpdate();
         
        // Draw Ghosts
        ConstructionManager.DrawGhosts();

        // Do Batch Rendering
        FloorTileRenderer.Render();
        BatchRenderer.Render();
        ItemRenderer.Tick(); 
         
        // Call frame updates
        Structure.FrameUpdateAllStructures();
        RoverManager.FrameUpdateRovers();
    }

    private void FixedUpdate()
    {
        if (GameWorld == null) { return; }

        GameWorld.OnFixedUpdate();
         
        ChainManager.UpdateChains(); 
        RoverManager.TickRovers(); 
        Structure.TickAllStructures(); 
        Electrical.SystemManager.Tick();

        BatchRenderer?.Tick();
    }

    public void CreateNewGame(string saveName)
    {
        DestroyMainMenuCamera();
        AudioManager.StopMusic();

        // User input save file name

        // Random seed is chosen
        int seed = UnityEngine.Random.Range(0, int.MaxValue);

        // Create new gameWorld
        GameWorld = new GameWorld(seed);
        GameLighting.SetActive(true);

        // Initialise Game Managers Components
        ScienceManager = new();
        ScienceManager.SetupNewGame(); 

        ConstructionManager = new(); 
        TaskManager = new();
        RoverManager = new();
        ElectricalSystemManager = new();
/*
        // Start zone is generated
        GameWorld.GenerateStartZone();*/

        // Descent vehicle animation plays
        Instantiate(RenderData.LanderSequence);

        // UI startup animation plays
        HUDManager = Instantiate(MenuData.HUD, transform).GetComponent<HUDManager>();
        HUDManager.GetComponentInChildren<TechTreeController>();
        TutorialSequencer = HUDManager.GetComponentInChildren<TutorialSequencer>();

        // Tutorial toggle prompt

        // Save game to file

        // Enable player input
         
    }

    public void ExitToMenu()
    {
        // Save game to file 

        // Unload gameworld
        GameWorld = null; 
        GameLighting.SetActive(false);

        ScienceManager = null;
        ConstructionManager = null;
        TaskManager = null;
        RoverManager = null;
        ElectricalSystemManager = null;

        // Destroy HUD
        Destroy(HUDManager.gameObject);
        HUDManager = null;

        // Call exit action
        OnGameExit?.Invoke();

        // Load main menu scene
        OpenMainMenu();
    }

    public void PlaySplashScreen()
    { 
        Instantiate(SplashScreen);
        AudioManager.PlayMainMenuMusic();
    }

    public void OpenMainMenu()
    {  
        Instantiate(MenuData.mainMenu);
    }

    public void SpawnMainMenuCamera()
    { 
        if(_mainMenuCamera == null)
            _mainMenuCamera = Instantiate(MainMenuCameraPrefab, transform).GetComponent<Camera>(); 
    }

    public void DestroyMainMenuCamera()
    {
        if (_mainMenuCamera != null)
        {
            Destroy(_mainMenuCamera.gameObject);
            _mainMenuCamera = null;
        }
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
        int width = 512;
        int offset = 32;

        GameWorld.GenerateRegion(new(0 - offset, width*2 + offset), new(0, 80 + offset));

        StructureData drill = GetStructureData("StaticDrill"); 
        StructureData conveyor = GetStructureData("Conveyor"); 
        StructureData crusher = GetStructureData("Crusher"); 
        StructureData magSep = GetStructureData("ElectrostaticSeperator"); 
        StructureData hopper = GetStructureData("Hopper"); 
        StructureData pylon = GetStructureData("PowerPylon");
        StructureData solarpanel = GetStructureData("SolarPanel");
        StructureData dbugoutput = GetStructureData("DebugOutput");

        StructureData GetStructureData(string name) { return GlobalData.Structures.Find(structure => structure.name == name); }

        var position = new int2(0, 0);
        Structure SpawnStructure(StructureData structure)
        {
            return ConstructionManager.ForceSpawnStructure(position, 0, structure); 
        }

        for (int x = 0; x < width; x += 2)
        {
            position.x = x;
            position.y = offset;

            position.y -= 1; SpawnStructure(solarpanel); position.y += 1;

 
            SpawnStructure(dbugoutput);
            position.y += 1;
            var h = SpawnStructure(hopper);
            ((Hopper)h).QueueRequest(GlobalData.Resources.Find(r => r.name == "Regolith"));

            position.x += 1; SpawnStructure(pylon); position.x -= 1;
            position.y += 1;

            for (int i = 0; i < 4; i++)
            {
                SpawnStructure(conveyor); position.y += 1;
            }
            position.x += 1; position.y -= 1; SpawnStructure(pylon); position.y += 1; position.x -= 1;
            SpawnStructure(crusher); position.y += 1; 

            for (int i = 0; i < 16; i++)
            {
                SpawnStructure(conveyor); position.y += 1;
            }
            SpawnStructure(hopper); position.y += 1;

            for (int i = 0; i < 16; i++)
            {
                SpawnStructure(conveyor); position.y += 1;
            }
            SpawnStructure(hopper); position.y += 1;

            for (int i = 0; i < 16; i++)
            {
                SpawnStructure(conveyor); position.y += 1;
            }
            SpawnStructure(hopper); position.y += 1;

        }
/*
        for (int s = 0; s < width/2; s++)
        {
            DebugSpawnRover();
        }*/
    }

    static int roverSpawnOffset = 0;

    public void DebugSpawnRover()
    {
        RoverManager.SpawnNewWidget(new int2(roverSpawnOffset, 0));
        roverSpawnOffset++;
    }

    public void DebugUnlockAll()
    {
        foreach (StructureData i in GlobalData.Structures)
        {
            i.Unlock();
        }
        foreach (ResourceData i in GlobalData.Resources)
        {
            i.Unlock();
        }
/*        foreach (CraftingFormula i in GlobalData.CraftingFormulas)
        {
            i.Unlock();
        }*/
    }

    public void AddLifespanGizmo(Vector3 worldPosition, int lifespan)
    {
        BatchRenderer.gizmoRenderer.Add(worldPosition, lifespan);
    }

    public void ToggleInstantBuild()
    {
        DevFlags.InstantBuilding = !DevFlags.InstantBuilding;
    }
}