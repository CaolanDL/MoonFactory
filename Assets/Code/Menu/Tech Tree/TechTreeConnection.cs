using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTreeConnection : MonoBehaviour
{
    [SerializeField] TechTreeNode[] nextNodes = new TechTreeNode[0];
    TechTreeNode lastNode = null;

    public void Unlock()
    {
        foreach(var node in nextNodes)
        {
            node.ChangeState(TechTreeNode.State.Available);
        }
    }
}
