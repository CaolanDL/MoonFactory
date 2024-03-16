using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine; 

public class FloorTileRenderer : MonoBehaviour
{ 
    private Dictionary<FloorTileData, CachedMatrixArray> matrixArrays = new();

    CachedMatrixArray _matrixArray; 

    static int maxCachedArrays = 16;

    public int tilesRenderedThisFrame = 0;

    private CameraController cameraController;

    RenderParams renderParams;
     
    private void Awake()
    {
        cameraController = GetComponent<CameraController>();

        renderParams = new RenderParams(GameManager.Instance.GlobalData.mat_Tile);  
    }

    public void Init()
    {
        foreach (FloorTileData tileData in WorldGenerationData.Instance.floorTiles)
        {
            if (matrixArrays.ContainsKey(tileData)) { continue; }

            matrixArrays.Add(tileData, new CachedMatrixArray(maxCachedArrays));
        } 
    }

    public void Tick()
    {
        DrawVisibleFloorTiles();
    }

    // Cached instanced to improve garbage collection perf

    GameWorld gameWorld;

    FloorTile currentFloorTile;


    int2 tileLocation;

    int2 xVisibleRange;

    int2 yVisibleRange;


    void DrawVisibleFloorTiles()
    { 
        (xVisibleRange, yVisibleRange) = cameraController.GetVisibleRange();

        gameWorld = GameManager.Instance.gameWorld;

        tileLocation = new int2();

        tilesRenderedThisFrame = 0;

        for (int x = xVisibleRange.x; x < xVisibleRange.y; x++)
        {
            for (int y = yVisibleRange.x; y < yVisibleRange.y; y++)
            {
                (tileLocation.x, tileLocation.y) = (x, y); 

                currentFloorTile = (FloorTile)gameWorld.floorGrid.GetEntityAt(tileLocation); 

                if (currentFloorTile == null) { continue; }

                //DrawTile(currentFloorTile); //Deprecated

                QueueTile();
                tilesRenderedThisFrame++;
            }
        }

        if (matrixArrays.Count > 0)
        {
            RenderTiles();
        }
    }

    void RenderTiles()
    {  
        foreach (FloorTileData tileData in matrixArrays.Keys)
        {
            _matrixArray = matrixArrays[tileData];

            for(int chunkIndex = 0; chunkIndex < maxCachedArrays; chunkIndex++)
            { 
                if (chunkIndex == _matrixArray.chunkIndex)
                {
                    Graphics.DrawMeshInstanced(tileData.mesh, 0, GlobalData.Instance.mat_Tile, _matrixArray.matrices[chunkIndex], _matrixArray.itemIndex);
                    break;
                }
                else
                {
                    Graphics.DrawMeshInstanced(tileData.mesh, 0, GlobalData.Instance.mat_Tile, _matrixArray.matrices[chunkIndex]);
                }
            }

            _matrixArray.Reset();
        }
    }

    void QueueTile()
    {
        if (currentFloorTile == null) return; 

        matrixArrays[currentFloorTile.data].QueueMatrix(currentFloorTile.transform.ToMatrix());
    }  

    // Deprecated

    void DrawTile(FloorTile floorTile)
    {
        Graphics.RenderMesh(renderParams, floorTile.data.mesh, 0, floorTile.transform.ToMatrix());
    }

    void DrawTile(int2 gridPosition, FloorTileData tileData)
    {
        Vector3 worldPosition = new Vector3(gridPosition.x, 0, gridPosition.y);

        Graphics.DrawMesh(tileData.mesh, worldPosition, quaternion.identity, GlobalData.Instance.mat_Tile, 0);
    } 

}


