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

    int itemCount;
    TextMeshProUGUI ItemCounter;

    int tileInWorldCount;
    TextMeshProUGUI TileInWorldCounter;
    int tileOnScreenCount;
    TextMeshProUGUI TileOnScreenCounter;

    private void Start()
    {
        FPSCounter = NewTextLine();
        ItemCounter = NewTextLine();
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

        itemCount = 0;
        foreach (Chain chain in ChainManager.chains) itemCount += chain.items.Count;
        ItemCounter.text = $"Items: {itemCount}";

        if(GameManager.Instance.gameWorld != null)
        {
            tileInWorldCount = GameManager.Instance.gameWorld.floorGrid.grid.Count;
        }
        TileInWorldCounter.text = $"Tiles in world: {tileInWorldCount}";
        if (GameManager.Instance.gameWorld != null)
        {
            tileOnScreenCount = GameManager.Instance.floorTileRenderer.tilesRenderedThisFrame;
        }
        TileOnScreenCounter.text = $"Tiles on screen: {tileOnScreenCount}";
    }
}
