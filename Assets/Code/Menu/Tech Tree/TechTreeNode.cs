using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TechTreeNode : MonoBehaviour
{
    [SerializeField] TechTreeNode[] nextNodes = new TechTreeNode[0];
    TechTreeNode lastNode = null;
    public bool Locked = true;

    // Start is called before the first frame update
    void Start()
    {
        foreach (var node in nextNodes)
        {
            node.lastNode = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    } 
}
