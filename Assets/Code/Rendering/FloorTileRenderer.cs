using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UIElements;

public class FloorTileRenderer : MonoBehaviour
{
    [SerializeField] private CameraController cameraController;

    GameWorld gameWorld;


    private void Awake()
    {
        cameraController = GetComponent<CameraController>();
    }
    public void Tick()
    {
        gameWorld = GameManager.Instance.gameWorld;
        DrawVisibleFloorTiles();
    }


    void DrawVisibleFloorTiles()
    {
        (int2 xVisibleRange, int2 yVisibleRange) = GetVisibleRange();


        for (int x = xVisibleRange.x; x < xVisibleRange.y; x++)
        {
            for (int y = yVisibleRange.x; y < yVisibleRange.y; y++)
            {

                int2 tileLocation = new int2(x, y);

                //Debug.Log($"Drawing Floor Tiles at: {x}, {y}");

                FloorTile floorTile = (FloorTile)gameWorld.floorGrid.GetEntityAt(tileLocation);


                // Generate new floor tiles
                if (floorTile == null) 
                {
                    floorTile = GameManager.Instance.gameWorld.GenerateFloorTile(tileLocation);
                }

                //UnityEngine.Debug.Log(gameWorld.floorGrid.GetEntityAt(tileLocation).GetType().ToString()) ;

                DrawTile(tileLocation, floorTile.tileData);
            }
        }
    } 


    void DrawTile(int2 gridPosition, FloorTileData tileData)
    {
        Vector3 worldPosition = new Vector3(gridPosition.x, 0, gridPosition.y);

        //Debug.Log(worldPosition);

        Graphics.DrawMesh(tileData.mesh, worldPosition, quaternion.identity, tileData.material, 0);
    } 


    (int2 xVisibleRange, int2 yVisibleRange) GetVisibleRange()
    {
        float2 cameraPosition = new float2(cameraController.position.x, cameraController.position.z);

        float cameraZoom = cameraController.zoom;

        int xSize = (int)(8 * cameraZoom); // Need to include screen aspect ratio compensation;
        int ySize = (int)(8 * cameraZoom); // Currently defaulting to 16:9

        int2 xRangeOut = new int2((int)cameraPosition.x - xSize / 2, (int)cameraPosition.x + xSize / 2 + 1);
        int2 yRangeOut = new int2((int)cameraPosition.y - ySize / 2, (int)cameraPosition.y + ySize / 2 + 1);

        return (xRangeOut, yRangeOut);
    }

}