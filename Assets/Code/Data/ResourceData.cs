using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Resource Data")]
public class ResourceData : ScriptableObject
{ 
    public string description;
    public Sprite sprite;
    public Mesh mesh;
    public Material material;
}
