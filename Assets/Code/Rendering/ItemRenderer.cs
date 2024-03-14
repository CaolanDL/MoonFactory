using Logistics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using ExtensionMethods;
 
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
            bool shouldRender = false;
            foreach (Conveyor conveyor in chain.conveyors) // Check to see if any conveyor within the chain is within the visible range
            {
                if (conveyor.position.WithinRange(xVisibleRange) && conveyor.position.y.WithinRange(yVisibleRange))
                {
                    shouldRender = true;
                    break;
                }
            }
            if (shouldRender == false) continue;
            foreach(Item item in chain.items) // Render any item that is within the visible range
            { 
                if (item.worldPosition.x.WithinRange(xVisibleRange) && item.worldPosition.y.WithinRange(yVisibleRange))  
                {
                    serialItems.Add(new SerialItem(item.worldPosition, item.distance, chain.items.IndexOf(item)));
                    DrawItem(item, item.worldPosition, item.Rotation); 
                }
            }
        }
         
    }


    void DrawItem(Item item, Vector2 position, short rotation)
    {
        Vector3 worldPosition = new Vector3(position.x, VerticalOffset, position.y); 

        // You should migrate to Graphics.RenderMesh as this function is now obsolete.
        Graphics.DrawMesh(item.data.mesh, worldPosition, Quaternion.Euler(0, rotation, 0), GlobalData.Instance.mat_DevUniversal, 0);
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