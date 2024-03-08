using Logistics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
 
public class ItemRenderer : MonoBehaviour
{
    private CameraController cameraController;

    [SerializeField] public float VerticalOffset = 0.25f;

    private void Awake()
    {
        cameraController = GetComponent<CameraController>();
    }

    public void Tick()
    {
        DrawVisibleItems();
    }

    [SerializeField] List<SerialItem> serialItems;

    void DrawVisibleItems()
    {
        (int2 xVisibleRange, int2 yVisibleRange) = GetVisibleRange();

        serialItems = new();

        foreach (Chain chain in ChainManager.chains)
        {
            foreach(Item item in chain.items)
            { 
                if (true) //itemPosition.x > xVisibleRange.x && itemPosition.x < xVisibleRange.y &&
                     //itemPosition.y > xVisibleRange.x && itemPosition.y < xVisibleRange.y)
                {
                    serialItems.Add(new SerialItem(item.worldPosition, item.distance, chain.items.IndexOf(item)));
                    DrawItem(item, item.worldPosition, item.GetConveyor(chain).rotation); 
                }
            }
        }

        //Debug.Log($"Items Rendered: {itemsRendered}");
    }


    void DrawItem(Item item, Vector2 position, sbyte rotation)
    {
        Vector3 worldPosition = new Vector3(position.x, VerticalOffset, position.y); 

        Graphics.DrawMesh(item.data.mesh, worldPosition, Quaternion.Euler(0, 90 * rotation, 0), GlobalData.Instance.mat_DevUniversal, 0);
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


    [Serializable]
    public class SerialItem
    {
        [SerializeField] public float2 worldPosition;
        [SerializeField] public int distance;
        [SerializeField] public int index;

        public SerialItem(float2 position, int distance, int index)
        {
            this.worldPosition = position;
            this.distance = distance;
            this.index = index;
        }
    }
} 