using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class FloorTileRenderer : MonoBehaviour
{
    private Dictionary<FloorTileData, ChunkedMatrixArray> matrixArrays = new();

    ChunkedMatrixArray _matrixArray;

    //static int maxCachedArrays = 16;

    public int tilesRenderedThisFrame = 0;

    private CameraController3D cameraController;

    RenderParams renderParams;

    private void Awake()
    {
        cameraController = GameManager.Instance.CameraController;

        renderParams = new RenderParams(GameManager.Instance.GlobalData.TerrainMaterial);
    }

    public void Init()
    {
        foreach (FloorTileData tileData in TerrainGenerationData.Instance.tileRegistry)
        {
            if (matrixArrays.ContainsKey(tileData)) { continue; }

            matrixArrays.Add(tileData, new ChunkedMatrixArray());
        }
    }

    public void Render()
    {
        DrawVisibleFloorTiles();
    }

    // Cached instanced to improve garbage collection perf <- This is non factual I think, Still Learning how this gets compiled.
    // Pretty sure this compiles exactly the same as using var in the loop.

    GameWorld gameWorld; 
    FloorTile currentFloorTile; 
    int2 tileLocation; 
    int2 xVisibleRange; 
    int2 yVisibleRange; 

    void DrawVisibleFloorTiles()
    {
        var VisibleRange = cameraController.GetLocalVisibleRange();

        int2 camGridPos = cameraController.CameraGridPosition;

        gameWorld = GameManager.Instance.GameWorld;

        tileLocation = new int2();

        tilesRenderedThisFrame = 0;

        for (int x = VisibleRange.x - 1; x < VisibleRange.y + 1; x++)
        {
            for (int y = VisibleRange.x - 1; y < VisibleRange.y + 1; y++)
            {
                for (int j = 0; j < 2; j++)
                { 
                    (tileLocation.x, tileLocation.y) = (camGridPos.x + (x - y + j) , camGridPos.y + (y + x + 1) ); 

                    var currentEntity = gameWorld.floorGrid.GetEntityAt(tileLocation); 
                    if (currentEntity is null || currentEntity is not FloorTile) { continue; } 
                    currentFloorTile = (FloorTile)currentEntity; 
                    
                    QueueTile();
                    tilesRenderedThisFrame++;
                } 
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

            for (int chunkIndex = 0; chunkIndex <= _matrixArray.chunkIndex; chunkIndex++)
            {
                if(_matrixArray.totalInQueue == 0) { continue; }

                if (chunkIndex == _matrixArray.chunkIndex)
                {
                    //Graphics.DrawMeshInstanced(tileData.mesh, 0, GlobalData.Instance.mat_Tile, _matrixArray.matriceChunks[chunkIndex], _matrixArray.itemIndex);
                    Graphics.RenderMeshInstanced(renderParams, tileData.mesh, 0, _matrixArray.matriceChunks[chunkIndex], _matrixArray.itemIndex+1);
                    break;
                }
                else
                {
                    //Graphics.DrawMeshInstanced(tileData.mesh, 0, GlobalData.Instance.mat_Tile, _matrixArray.matriceChunks[chunkIndex]);
                    Graphics.RenderMeshInstanced(renderParams, tileData.mesh, 0, _matrixArray.matriceChunks[chunkIndex]);
                }
            }

            _matrixArray.Reset();
        }
    }

    void QueueTile()
    {
        if (currentFloorTile == null) return; 

        matrixArrays[currentFloorTile.data].QueueMatrix(currentFloorTile.transform);
    }

    // Deprecated

    void DrawTile(FloorTile floorTile)
    {
        Graphics.RenderMesh(renderParams, floorTile.data.mesh, 0, floorTile.transform.ToMatrix());
    }

    void DrawTile(int2 gridPosition, FloorTileData tileData)
    {
        Vector3 worldPosition = new Vector3(gridPosition.x, 0, gridPosition.y);

        Graphics.DrawMesh(tileData.mesh, worldPosition, quaternion.identity, GlobalData.Instance.TerrainMaterial, 0);
    }

}


