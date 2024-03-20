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

    private float VerticalOffset = 0.205f;

    private CameraController cameraController;

    private void Awake()
    {
        cameraController = GetComponent<CameraController>();
    }

    public void Init()
    {
        foreach (ResourceData data in GlobalData.Instance.resources)
        {
            if (matrixArrays.ContainsKey(data)) { continue; }

            matrixArrays.Add(data, new ChunkedMatrixArray());
        }
    }

    public void Tick()
    {
        DrawVisibleItems();
    } 

    int2 xVisibleRange; int2 yVisibleRange;

    public int itemsRenderedThisFrame = 0; 

    static Vector2 itemV2WorldPosCached; 
    static short itemRotationCached;

    void DrawVisibleItems()
    {
        (xVisibleRange, yVisibleRange) = cameraController.GetDiamondVisibleRange();
        itemsRenderedThisFrame = 0;

        foreach (Chain chain in ChainManager.chains)
        {
            bool shouldRender = false;
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
                    QueueItem(item) ;
                }
            }
        }

        RenderItems(); 
    }

    void RenderItems()
    {
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


    void QueueItem(Item item)
    {
        if (item == null) return;

        Vector3 _worldPosition = new Vector3(itemV2WorldPosCached.x, VerticalOffset, itemV2WorldPosCached.y); 

        var _matrix = Matrix4x4.TRS(
            _worldPosition,
            Quaternion.Euler(0, itemRotationCached,0),
            Vector3.one); 

        matrixArrays[item.data].QueueMatrix(_matrix);
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