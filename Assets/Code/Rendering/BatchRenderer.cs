using UnityEngine;

public class BatchRenderer : MonoBehaviour
{
    WireRenderer wireRenderer = new();

    public void Init()
    {
        wireRenderer.Init();
    }

    public void Tick()
    {
        wireRenderer.RenderAll();
    }

    public static void RenderChunkedMatrixArray(ChunkedMatrixArray chunkedMatrixArray, Mesh mesh, Material material)
    {
        for (int chunkIndex = 0; chunkIndex <= chunkedMatrixArray.matriceChunks.Count; chunkIndex++)
        {
            if (chunkIndex == chunkedMatrixArray.chunkIndex)
            {
                Graphics.DrawMeshInstanced(mesh, 0, material, chunkedMatrixArray.matriceChunks[chunkIndex], chunkedMatrixArray.itemIndex);
                break;
            }
            else
            {
                Graphics.DrawMeshInstanced(mesh, 0, material, chunkedMatrixArray.matriceChunks[chunkIndex]);
            }
        }

        chunkedMatrixArray.Reset();
    }
}

public class WireRenderer
{
    ChunkedMatrixArray chunkedMatrixArray = new();
    public int RenderCount = 0;

    public Mesh mesh;
    public Material material;

    public void Init()
    {
        mesh = GameManager.Instance.RenderData.wire_mesh;
        material = GameManager.Instance.RenderData.wire_material;
    }

    public void RenderAll()
    {
        foreach(Electrical.Connection connection in Electrical.Connection.pool)
        {
            var matrix = Matrix4x4.TRS(connection.origin, connection.rotation, connection.scale);

            chunkedMatrixArray.QueueMatrix(matrix);
        }

        BatchRenderer.RenderChunkedMatrixArray(chunkedMatrixArray, mesh, material);
    }

    public void RenderVisible()
    {

    }
}