using System.Collections.Generic;
using UnityEngine;

public class TechTreeController : MonoBehaviour
{
    [SerializeField] TreeNode FirstNode;

    [Header("Node Colors")]
    [SerializeField] public Color Unlocked;
    [SerializeField] public Color Available;
    [SerializeField] public Color Locked;

    [Header("Connection Colors")]
    [SerializeField] public Color Unlocked_Connection; 
    [SerializeField] public Color Locked_Connection;

    public Dictionary<TreeNode.State, Color> StateColors;

    private void Awake()
    {
        StateColors = new()
        {
            { TreeNode.State.Unlocked, Unlocked },
            { TreeNode.State.Available, Available },
            { TreeNode.State.Locked, Locked }
        }; 
    }

    private void Start()
    {
        FirstNode.ChangeState(TreeNode.State.Available);
        FirstNode.TryUnlock();
    }
}
