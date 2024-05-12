﻿using System.Collections;
using UnityEngine;


public class Lander : Structure
{
    public float powerProduction = 50f;   
    public Electrical.Relay ElectricalRelay; 
    public Inventory inventory = new Inventory();

    public override bool CanDemolish()
    {
        return false;
    }

    public override void OnFrameUpdate()
    {
        base.OnFrameUpdate();

        if(TutorialProxy.IsActive)
        {
            TutorialProxy.SetPopupPosition?.Invoke(GameManager.Instance.CameraController.activeMainCamera.WorldToScreenPoint(DisplayObject.transform.position), TutorialTag.LanderPosition);
        }
    }

    public override void OnInitialise()
    {
        base.OnInitialise(); 

        AddStartingResources();

        SupplyPort = new SupplyPort(this);
        SupplyPort.AddInventory(inventory);

        ElectricalNode = new Electrical.Input();
        ((Electrical.Input)ElectricalNode).Production = powerProduction;

        ElectricalRelay = new()
        {
            Parent = this
        };
    }

    public override void OnConstructed()
    {
        base.OnConstructed();
        ElectricalRelay.Constructed();
    }

    public override void OnDemolished()
    {
        base.OnDemolished();
        ElectricalRelay.Demolished();
    }

    void AddStartingResources()
    {
        inventory.maxItems = 10000; 

        foreach (var rq in GlobalData.Instance.StarterResources)
        {
            inventory.TryAddResource(rq);
        }
    }

    public override void OnClicked(Vector3 mousePosition)
    {
        OpenInterfaceOnHUD(MenuData.Instance.LanderInterface, mousePosition);
    }
}