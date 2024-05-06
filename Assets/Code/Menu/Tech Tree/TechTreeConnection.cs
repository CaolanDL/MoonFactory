using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TechTreeConnection : MonoBehaviour
{
    TechTreeController techTree;

    [SerializeField] TechTreeNode[] nextNodes = new TechTreeNode[0]; 
    [SerializeField] List<Image> SubConnections = new();

    TechTreeNode lastNode = null;

    private void Awake()
    {
        techTree = GetComponentInParent<TechTreeController>();
        SetColor(techTree.Locked_Connection);
    }

    public void Unlock()
    {
        foreach(var node in nextNodes)
        {
            node.ChangeState(TechTreeNode.State.Available);
        }

        SetColor(techTree.Unlocked_Connection);
    }

    public void SetColor(Color color)
    {
        GetComponent<Image>().color = color;
        foreach (var connection in SubConnections)
        {
            connection.color = color;
        }
    }
}
