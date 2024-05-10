using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticDrillInterface : StaticInterface
{
    StaticDrill staticDrill;

    [SerializeField] InventoryElement inventory;

    public override void Init(Entity entity, Vector3 screenPosition)
    {
        base.Init(entity, screenPosition);

        staticDrill = (StaticDrill)entity;

        inventory.linkedInventory = staticDrill.inventory;
    }

    private void Update()
    {
        UpdateUI();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();

        inventory.UpdateDetails();
    }
}
