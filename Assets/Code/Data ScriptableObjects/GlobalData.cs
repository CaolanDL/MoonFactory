using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Singletons/Global Data")]
public class GlobalData : ScriptableObject
{ 
    public static GlobalData Instance { get; private set; }

    private void OnEnable()
    {
        MakeSingleton();
    }

    public void MakeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    [Header("Development Assets")]
    public Material mat_DevUniversal;

    [Header("Global Materials")]
    public Material mat_ghost;
    public Material mat_ghostBlocked;

    [Header("Indicator Arrow Data")]
    public Mesh m_ArrowIndicator;
    public Material mat_ArrowIndicator;


    [Header("Structure Registry")]
    public List<StructureData> structures;
}
