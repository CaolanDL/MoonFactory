using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class StructureTooltip : MonoBehaviour
{
    StructureData structure;

    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;
    public GameObject CraftablesHeader;
    public Transform CraftablesContainer;
    public Transform RequirmentsContainer;

    [SerializeField] GameObject ResourceIconPrefab;
    [SerializeField] GameObject ResourceIconCountPrefab; 

    private void Start()
    {
        SetPosition();
    }

    public void SetDetails(StructureData structureData)
    {
        structure = structureData;

        Title.SetText(structureData.screenname); 
        Description.SetText(structureData.description);

        if(structure.CraftableResources.Count == 0)
        {
            Destroy(CraftablesContainer.gameObject);
            Destroy(CraftablesHeader);
        }
        else
        {
            foreach (var craftable in structure.CraftableResources)
            {
                var icon = Instantiate(ResourceIconPrefab, CraftablesContainer);
                icon.GetComponent<ResourceIcon>()?.SetDetails(craftable); 
            }
        } 
        foreach (var rq in structure.requiredResources)
        {
            var icon = Instantiate(ResourceIconCountPrefab, RequirmentsContainer);
            icon.GetComponent<ResourceIcon>()?.SetDetails(rq.resource);
            icon.GetComponent<ResourceIcon>()?.SetCount(rq.quantity); 
        }

        SetPosition(); 
    }

    private void Update()
    {
        SetPosition();
    }

    void SetPosition()
    {
        var mousePos = Mouse.current.position.value;
        transform.position = new Vector3(mousePos.x, mousePos.y, 0);

        Canvas.ForceUpdateCanvases();
    }
}
