using System.Collections.Generic;
using UnityEngine;

public class ChunkedMatrixArray
{
    public List<Matrix4x4[]> matrices;

    public byte chunkIndex = 0;
    public int itemIndex = 0; 

    public ChunkedMatrixArray()
    { 
        matrices = new();

        CacheNewArray();
    }

    public void QueueMatrix(Matrix4x4 matrix)
    {
        matrices[chunkIndex][itemIndex] = matrix;

        itemIndex++;

        if (itemIndex == 1024)
        {
            chunkIndex++;
            itemIndex = 0;

            if(chunkIndex > matrices.Count - 1)
            {
                CacheNewArray();
            }
        }
    }

    void CacheNewArray()
    {
        matrices.Add(new Matrix4x4[1024]);
    }

    public void Reset()
    {
        chunkIndex = 0;
        itemIndex = 0;
    }
}