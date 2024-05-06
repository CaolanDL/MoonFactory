using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StructureTooltip : MonoBehaviour
{
    StructureData structure;

    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;
    public Transform CraftablesContainer;

    [SerializeField] GameObject ResourceIconPrefab;
    List<GameObject> craftablesIcons = new();

    private void Start()
    {
        SetPosition();
    }

    public void SetDetails(StructureData structureData)
    {
        structure = structureData;

        Title.SetText(structureData.screenname); 
        Description.SetText(structureData.description);

        foreach(var craftable in structure.CraftableResources)
        {
            var icon = Instantiate(ResourceIconPrefab, CraftablesContainer);
            icon.GetComponent<ResourceIcon>()?.SetDetails(craftable);
            craftablesIcons.Add(icon);
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
