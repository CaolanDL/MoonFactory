using Logistics;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine; 

public class DebugUI : MonoBehaviour
{
    [SerializeField] GameObject VerticalLayout;
    [SerializeField] GameObject TextLinePrefab;

    int FPS;
    TextMeshProUGUI FPSCounter;

    int itemInWorldCount;
    TextMeshProUGUI ItemsInWorldCounter;   
    TextMeshProUGUI ItemsRenderedThisFrameCounter;

    int tileInWorldCount;
    TextMeshProUGUI TileInWorldCounter;
    int tileOnScreenCount;
    TextMeshProUGUI TileOnScreenCounter;

    private void Awake()
    {
/*        #if !UNITY_EDITOR
        Destroy(gameObject);
        #endif*/
    }

    private void Start()
    {
        FPSCounter = NewTextLine();
        ItemsInWorldCounter = NewTextLine();
        ItemsRenderedThisFrameCounter = NewTextLine();
        TileInWorldCounter = NewTextLine();
        TileOnScreenCounter = NewTextLine();
    }

    TextMeshProUGUI NewTextLine() { return Instantiate(TextLinePrefab, VerticalLayout.transform).GetComponent<TextMeshProUGUI>(); }

    private void Update()
    {
        FPS = (int)(1.0f / Time.smoothDeltaTime);
    }

    private void FixedUpdate()
    {
        FPSCounter.text = $"FPS {FPS}";

        itemInWorldCount = 0;
        foreach (Chain chain in ChainManager.chains) itemInWorldCount += chain.items.Count;
        ItemsInWorldCounter.text = $"Items in world: {itemInWorldCount}";

        ItemsRenderedThisFrameCounter.text = $"Items rendered this frame: {GameManager.Instance.itemRenderer.itemsRenderedThisFrame}";

        if (GameManager.Instance.gameWorld != null)
        {
            tileInWorldCount = GameManager.Instance.gameWorld.floorGrid.grid.Count;
        }
        TileInWorldCounter.text = $"Tiles in world: {tileInWorldCount}";
        if (GameManager.Instance.gameWorld != null)
        {
            tileOnScreenCount = GameManager.Instance.floorTileRenderer.tilesRenderedThisFrame;
        }
        TileOnScreenCounter.text = $"Tiles rendered this frame: {tileOnScreenCount}";
    }
}
