using System.Collections;
using UnityEngine;
 
public class AssetWarmer
{
    GameManager gameManager = GameManager.Instance;
    RenderData renderData = RenderData.Instance;
    GlobalData globalData = GlobalData.Instance;

    Matrix4x4 TRS = Matrix4x4.TRS(new Vector3(), Quaternion.identity, Vector3.zero);

    public void Warmup()
    {  
        foreach(var item in gameManager.GlobalData.Resources) 
            Graphics.DrawMesh(item.mesh, TRS, item.material, 0);

        foreach (var item in gameManager.GlobalData.Structures)
            Graphics.DrawMesh(item.ghostMesh, TRS, RenderData.Instance.UniversalMaterial, 0);

        WarmupDraw(renderData.Arrow, renderData.ArrowInputMaterial);
        WarmupDraw(renderData.Arrow, renderData.ArrowOutputMaterial);

        WarmupDraw(renderData.Arrow, renderData.wire_material);
        WarmupDraw(renderData.Arrow, renderData.devgizmo_material);
        WarmupDraw(renderData.Arrow, renderData.BlueGizmoMaterial);
        WarmupDraw(renderData.Arrow, renderData.TransparentBlueGizmoMaterial);
        WarmupDraw(renderData.Arrow, renderData.UniversalMaterial);
        WarmupDraw(renderData.Arrow, globalData.mat_Tile);  
    }

    void WarmupDraw(Mesh mesh, Material material)
    {
        Graphics.DrawMesh(mesh, TRS, material, 0);
    }
}