using System.Collections.Generic;
using UnityEngine;

public class ChunkedMatrixArray
{
    public List<Matrix4x4[]> matriceChunks = new(32);

    public byte chunkIndex = 0;
    public int itemIndex = 0;

    public ChunkedMatrixArray()
    { 
        matriceChunks = new(32);

        for(int i = 0; i < 32; i++)
        {
            CacheNewArray();
        } 
    }

    public void QueueMatrix(Matrix4x4 matrix)
    {
        matriceChunks[chunkIndex][itemIndex] = matrix;

        itemIndex++;

        if (itemIndex == 1024)
        {
            chunkIndex++;
            itemIndex = 0;

            if(chunkIndex > matriceChunks.Count - 1)
            {
                CacheNewArray();
            }
        }
    }

    void CacheNewArray()
    {
        matriceChunks.Add(new Matrix4x4[1024]);
    }

    public void Reset()
    {
        chunkIndex = 0;
        itemIndex = 0;
    }
}