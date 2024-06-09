using Logistics;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

using ExtensionMethods;
 
public class ItemRenderer : MonoBehaviour
{  
    private Dictionary<ResourceData, ChunkedMatrixArray> matrixArrays = new();

    ChunkedMatrixArray _matrixArray; 

    private float VerticalOffset = 0.21f;

    private CameraController cameraController;

    private void Awake()
    {
        cameraController = GetComponent<CameraController>();
    }

    public void Init()
    {
        foreach (ResourceData data in GlobalData.Instance.Resources)
        {
            if (matrixArrays.ContainsKey(data)) { continue; }

            matrixArrays.Add(data, new ChunkedMatrixArray());
        }
    }

    public void Tick()
    {
        RenderVisibleItems();
    } 

    int2 xVisibleRange; int2 yVisibleRange;

    public int itemsRenderedThisFrame = 0; 

    static Vector2 itemV2WorldPosCached; 
    static short itemRotationCached;

    void RenderVisibleItems()
    {
        var visibleRange = GameManager.Instance.CameraController.GetLocalVisibleRange();
        int2 camGridPos = GameManager.Instance.CameraController.CameraGridPosition;

        xVisibleRange = new int2(camGridPos.x + visibleRange.x, camGridPos.x + visibleRange.y);
        yVisibleRange = new int2(camGridPos.y + visibleRange.x, camGridPos.y + visibleRange.y);
         
        itemsRenderedThisFrame = 0;

        // Loop through each conveyor chain and identify if any part of the chain is within the visible area
        //? Probably a more effective way to do this: Search each grid location within the visible range for a conveyor, then retrieve the items on that conveyor, then render those items.
        //? This way the only conveyors, chains and items that are iterated over are the ones that will ultimately be rendered.
        // The existing implementation is performant enough, only make this change if you start to experience slow downs with large scale game worlds.
        foreach (Chain chain in ChainManager.chains)
        {
            bool shouldRender = true;
            foreach (Conveyor conveyor in chain.conveyors) // Check to see if any conveyor within the chain is within the visible range
            {
                if (conveyor.position.x.WithinRange(xVisibleRange) && conveyor.position.y.WithinRange(yVisibleRange))
                {
                    shouldRender = true;
                    break;
                }
            }
            if (shouldRender == false) continue;
            foreach(Item item in chain.items) // Render any item that is within the visible range
            {
                itemV2WorldPosCached = item.GetWorldPosition(chain);

                if (itemV2WorldPosCached.x.WithinRange(xVisibleRange) && itemV2WorldPosCached.y.WithinRange(yVisibleRange))  
                {
                    itemRotationCached = item.GetRotation();

                    itemsRenderedThisFrame++;
                    QueueItem(item);
                }
            }
        }

        // Loop through each matrice chunk and send its data to the GPU
        foreach (ResourceData resourceData in matrixArrays.Keys)
        {
            _matrixArray = matrixArrays[resourceData];

            for (int chunkIndex = 0; chunkIndex <= _matrixArray.matriceChunks.Count; chunkIndex++)
            {
                if (chunkIndex == _matrixArray.chunkIndex)
                {
                    Graphics.DrawMeshInstanced(resourceData.mesh, 0, resourceData.material, _matrixArray.matriceChunks[chunkIndex], _matrixArray.itemIndex);
                    break;
                }
                else
                {
                    Graphics.DrawMeshInstanced(resourceData.mesh, 0, resourceData.material, _matrixArray.matriceChunks[chunkIndex]);
                }
            }

            _matrixArray.Reset();
        }
    }  

    public void QueueItem(Item item)
    {
        if (item == null) return;

        Vector3 _worldPosition = new Vector3(itemV2WorldPosCached.x, VerticalOffset, itemV2WorldPosCached.y);  

        matrixArrays[item.data].QueueMatrix(_worldPosition, Quaternion.Euler(0, itemRotationCached, 0));
    }

    public void QueueItem(ResourceData resource, Matrix4x4 matrix)
    { 
        matrixArrays[resource].QueueMatrix(matrix);
    }

    void DrawItem(Item item, Vector2 position, short rotation)
    {
        Vector3 worldPosition = new Vector3(position.x, VerticalOffset, position.y); 

        // You should migrate to Graphics.RenderMesh as this function is now obsolete.
        Graphics.DrawMesh(item.data.mesh, worldPosition, Quaternion.Euler(0, rotation, 0), GlobalData.Instance.mat_DevUniversal, 0);
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