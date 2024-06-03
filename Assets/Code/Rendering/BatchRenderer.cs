using ExtensionMethods;
using Terrain;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using System;

public class BatchRenderer : MonoBehaviour
{
    WireRenderer wireRenderer = new();
    public GizmoRenderer gizmoRenderer = new();
    BridgeRenderer platformRenderer = new();
    public ItemRenderer ItemRenderer { get { return GameManager.Instance.ItemRenderer; } }
    ElectricalCoverageRenderer electricalCoverageRenderer = new();
    TerrainDetailsRenderer terrainRenderer = new();

    public void Init()
    {
        wireRenderer.Init();
        platformRenderer.Init();
    }

    public void Tick()
    {
        gizmoRenderer.Tick(); 
    }

    public void Render()
    {
        wireRenderer.RenderAll();
        gizmoRenderer.Render();
        platformRenderer.RenderAll();
        terrainRenderer.RenderVisible();

        electricalCoverageRenderer.TryRender();
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

public class TerrainDetailsRenderer
{
    private CameraController3D cameraController;
    private Dictionary<ModelData, ChunkedMatrixArray> meteorMatrixArrays = new();
    private Dictionary<Mesh, ChunkedMatrixArray> craterMatrixArrays = new();

    public void RenderVisible()
    {
        cameraController = GameManager.Instance.CameraController;
        var gameWorld = GameManager.Instance.GameWorld;

        var VisibleRange = cameraController.GetLocalVisibleRange();
        int2 camGridPos = cameraController.CameraGridPosition; 

/*        for (int x = VisibleRange.x - 2; x < VisibleRange.y + 2; x++)
        {
            for (int y = VisibleRange.x - 2; y < VisibleRange.y + 2; y++)
            {
                for (int j = 0; j < 2; j++)
                {
                    (tileLocation.x, tileLocation.y) = (camGridPos.x + (x - y + j), camGridPos.y + (y + x + 1));
                      
                    Meteorite meteorite = null;
                    if (gameWorld.meteorites.TryGetValue(tileLocation, out var m))
                    {
                        meteorite = m;
                    }
                    else continue;

                    var v = meteorite.position.ToVector3();
                    var r = meteorite.rotation.ToQuaternion();
                    var s = Vector3.one * meteorite.scale;
                    var matrix = Matrix4x4.TRS(v, r, s);

                    QueueModel(meteorite.modelData, matrix);
                }
            }
        }*/

        // Queue Visible Meteors
        ForIsometricRange((position) =>
        { 
            if (gameWorld.meteorites.TryGetValue(position, out var meteorite))
            { 
                var v = meteorite.position.ToVector3();
                var r = meteorite.rotation.ToQuaternion();
                var s = Vector3.one * meteorite.scale;
                var matrix = Matrix4x4.TRS(v, r, s);

                QueueMeteor(meteorite.modelData, matrix);
            }  
        });

        // Queue Visible Craters
        ForIsometricRange((position) =>
        { 
            if (gameWorld.craters.TryGetValue(position, out var crater))
            { 
                var v = crater.position.ToVector3();
                var r = crater.rotation.ToQuaternion();
                var s = Vector3.one;
                var matrix = Matrix4x4.TRS(v, r, s);

                QueueCrater(crater.craterData.mesh, matrix);
            }
        });


        void ForIsometricRange(Action<int2> action)
        {
            for (int x = VisibleRange.x; x < VisibleRange.y; x++)
            {
                for (int y = VisibleRange.x; y < VisibleRange.y; y++)
                {
                    for (int j = 0; j < 2; j++)
                    { 
                        action(new(camGridPos.x + (x - y + j), camGridPos.y + (y + x + 1)));
                    }
                }
            }
        }

        foreach (var kvp in meteorMatrixArrays)
        {
            BatchRenderer.RenderChunkedMatrixArray(kvp.Value, kvp.Key.Mesh, kvp.Key.Material);
        }
        foreach (var kvp in craterMatrixArrays)
        {
            BatchRenderer.RenderChunkedMatrixArray(kvp.Value, kvp.Key, GlobalData.Instance.TerrainMaterial);
        }
    }

    void QueueMeteor(ModelData modelData, Matrix4x4 matrix)
    {
        if (meteorMatrixArrays.ContainsKey(modelData) == false)
        {
            meteorMatrixArrays.Add(modelData, new());
        }

        meteorMatrixArrays[modelData].QueueMatrix(matrix);
    }
    void QueueCrater(Mesh mesh, Matrix4x4 matrix)
    {
        if (craterMatrixArrays.ContainsKey(mesh) == false)
        {
            craterMatrixArrays.Add(mesh, new());
        }

        craterMatrixArrays[mesh].QueueMatrix(matrix);
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
            if (connection.scale.magnitude < 0.1f) { continue; }
            var matrix = Matrix4x4.TRS(connection.origin, connection.rotation, connection.scale);

            chunkedMatrixArray.QueueMatrix(matrix);
        }

        BatchRenderer.RenderChunkedMatrixArray(chunkedMatrixArray, mesh, material);
    }

    public void RenderVisible()
    {

    }
}



public class GizmoRenderer
{
    List<Vector3> gizmoLocations = new();
    Dictionary<Vector3, int> gizmoLifespans = new();

    public void Add(Vector3 location, int lifespan)
    {
        if(gizmoLocations.Contains(location)) { return; }
        gizmoLocations.Add(location);
        gizmoLifespans.Add(location, lifespan);
    }

    public void Tick()
    {
        List<Vector3> gizmosToRemove = new();

        foreach(Vector3 location in gizmoLocations)
        {
            gizmoLifespans[location]--;
            if (gizmoLifespans[location] < 0) { gizmosToRemove.Add(location); }
        } 
        foreach(var item in gizmosToRemove)
        {
            gizmoLifespans.Remove(item);
            gizmoLocations.Remove(item);
        }
    } 

    public void Render()
    {
        foreach(var kvp in gizmoLifespans)
        {
            Graphics.DrawMesh(GlobalData.Instance.gizmo_Axis, kvp.Key, Quaternion.identity, GlobalData.Instance.mat_PulsingGizmo, 0);
        }
    } 
}


public class ElectricalCoverageRenderer
{
    List<Electrical.Relay> Relays { get { return Electrical.SystemManager.relays; } }
    ChunkedMatrixArray chunkedMatrixArray = new();

    public static bool enabled = false;

    public void TryRender()
    {
        if (enabled == false) return;

        List<Vector3> coveredPositions = new();

        foreach(var relay in Relays)
        {
            for (int x = -relay.connectionRange-1; x <= relay.connectionRange+1; x++)
            {
                for (int y = -relay.connectionRange-1; y <= relay.connectionRange+1; y++)
                { 
                    var pos = new Vector3(x + relay.Parent.position.x, 0.001f, y + relay.Parent.position.y);
                    if (coveredPositions.Contains(pos)) { continue; }
                    coveredPositions.Add(pos);
                    var matrix = Matrix4x4.TRS(pos, Quaternion.identity, Vector3.one); 
                    chunkedMatrixArray.QueueMatrix(matrix); 
                }
            }
        }

        BatchRenderer.RenderChunkedMatrixArray(chunkedMatrixArray, RenderData.Instance.TilePowerGizmo, RenderData.Instance.TilePowerGizmoMaterial);
    }
}

