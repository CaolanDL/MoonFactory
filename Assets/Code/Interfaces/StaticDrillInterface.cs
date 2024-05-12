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

        TutorialProxy.Action?.Invoke(TutorialEvent.StaticDrillInterfaceOpened);
    }

    public override void Update()
    {
        base.Update();

        UpdateUI();

        TutorialProxy.SetPopupPosition?.Invoke(transform.position, TutorialTag.StaticDrillInterfacePosition);
    }

    public override void UpdateUI()
    {
        base.UpdateUI();

        inventory.UpdateDetails();
    }

    public override void OnCloseInterface()
    {
        base.OnCloseInterface();

        TutorialProxy.Action?.Invoke(TutorialEvent.StaticDrillInterfaceClosed);
    }

    private void OnDisable()
    {
        TutorialProxy.Action?.Invoke(TutorialEvent.StaticDrillInterfaceClosed);
    }
}
