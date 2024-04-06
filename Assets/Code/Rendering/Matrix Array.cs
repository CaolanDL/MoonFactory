using System.Collections.Generic;
using UnityEngine;
using ExtensionMethods;
using System.Linq;

public class ChunkedMatrixArray
{
    public List<Matrix4x4[]> matriceChunks = new();

    public static int arraySize = 128;

    public byte chunkIndex = 0;
    public int itemIndex = 0;
    public int totalInQueue = 0;

    public ChunkedMatrixArray()
    {  
        for(int i = 0; i < 1; i++)
        {
            CacheNewArray();
        } 
    }

    public void QueueMatrix(Matrix4x4 matrix)
    {
        matriceChunks[chunkIndex][itemIndex] = matrix;

        itemIndex++;
        totalInQueue++;

        if (itemIndex == arraySize -1)
        {
            chunkIndex++;
            itemIndex = 0;

            if(chunkIndex > matriceChunks.Count - 1)
            {
                CacheNewArray();
            }
        }
    }

    public void QueueMatrix(TinyTransform tinyTransform)
    {
        matriceChunks[chunkIndex][itemIndex].SetTRS(tinyTransform.position.ToVector3(), tinyTransform.rotation.ToQuaternion(), Vector3.one);

        itemIndex++;
        totalInQueue++;

        if (itemIndex == arraySize - 1)
        {
            chunkIndex++;
            itemIndex = 0;

            if (chunkIndex > matriceChunks.Count - 1)
            {
                CacheNewArray();
            }
        }
    }

    public void QueueMatrix(Vector3 position, Quaternion rotation)
    {
        matriceChunks[chunkIndex][itemIndex].SetTRS(position, rotation, Vector3.one);

        itemIndex++;
        totalInQueue++;

        if (itemIndex == arraySize - 1)
        {
            chunkIndex++;
            itemIndex = 0;

            if (chunkIndex > matriceChunks.Count - 1)
            {
                CacheNewArray();
            }
        }
    }

    void CacheNewArray()
    {
        matriceChunks.Add(new Matrix4x4[arraySize]);

        var lastChunk = matriceChunks.Last();

        for (int i = 0; i < arraySize; i++)
        {
            lastChunk[i] = new Matrix4x4();
        }
    }

    public void Reset()
    {
        chunkIndex = 0;
        itemIndex = 0;
        totalInQueue = 0;
    }
}