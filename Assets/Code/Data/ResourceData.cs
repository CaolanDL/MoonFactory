using UnityEngine;

public enum ResourceCategory
{
    Basic,
    Refined,
    Manufactured
}

[CreateAssetMenu(menuName = "MoonFactory/Resource Data")]
public class ResourceData : ScriptableObject
{ 
    public string description;
    public ResourceCategory resourceCategory;

    [Header("Rendering")]
    //public Sprite sprite;
    public Mesh mesh;
    public Material material;

    public Sprite sprite;

    [Header("Details")]
    public byte weight = 1;
}
