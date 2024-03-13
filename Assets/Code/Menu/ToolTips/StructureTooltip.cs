using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class StructureTooltip : MonoBehaviour
{
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;

    private void Start()
    {
        SetPosition();
    }

    public void Init(StructureData structureData)
    {
        Title.SetText(structureData.screenname);

        Description.SetText(structureData.description);

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
