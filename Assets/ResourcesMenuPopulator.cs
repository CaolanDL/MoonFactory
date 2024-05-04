using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesMenuPopulator : MonoBehaviour
{
    [SerializeField] GameObject ResourceIcon;
    [SerializeField] GameObject empty;

    private void Awake()
    {
        foreach(var r in GameManager.Instance.GlobalData.Resources)
        {
            var newEmpty = Instantiate(empty, transform);
            var ri = Instantiate(ResourceIcon, newEmpty.transform);
            ri.GetComponent<ResourceIcon>()?.SetDetails(r);
        }
    }
}
