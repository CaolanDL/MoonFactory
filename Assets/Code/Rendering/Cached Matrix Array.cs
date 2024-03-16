using System.Collections.Generic;
using UnityEngine;

public class CachedMatrixArray
{
    public List<Matrix4x4[]> matrices;

    public byte chunkIndex = 0;
    public int itemIndex = 0;

    public int maxChunks;

    public CachedMatrixArray(int maxChunks)
    {
        this.maxChunks = maxChunks;

        matrices = new();

        for (int i = 0; i < maxChunks; i++)
        {
            matrices.Add(new Matrix4x4[1024]);
        }
    }

    public void QueueMatrix(Matrix4x4 matrix)
    {
        matrices[chunkIndex][itemIndex] = matrix;

        itemIndex++;

        if (itemIndex == 1024)
        {
            chunkIndex++;
            itemIndex = 0;
        }
    }

    public void Reset()
    {
        chunkIndex = 0;
        itemIndex = 0;
    }
}