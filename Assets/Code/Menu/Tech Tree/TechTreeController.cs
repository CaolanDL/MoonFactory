using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTreeController : MonoBehaviour
{
    [SerializeField] TechTreeNode FirstNode;

    [Header("Node Colors")]
    [SerializeField] public Color Unlocked;
    [SerializeField] public Color Available;
    [SerializeField] public Color Locked;

    public Dictionary<TechTreeNode.State, Color> StateColors;

    private void Awake()
    {
        StateColors = new()
        {
            { TechTreeNode.State.Unlocked, Unlocked },
            { TechTreeNode.State.Available, Available },
            { TechTreeNode.State.Locked, Locked }
        }; 
    }

    private void Start()
    {
        FirstNode.ChangeState(TechTreeNode.State.Available);
        FirstNode.TryUnlock();
    }
}
