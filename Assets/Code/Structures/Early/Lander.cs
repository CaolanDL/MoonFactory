﻿using System.Collections;
using UnityEngine;


//? Lander is not connecting to machines until a power pylon is placed next to it?

public class Lander : Structure
{
    public float powerProduction = 500f;   
    //public Electrical.Relay ElectricalRelay; 
    public Inventory inventory = new Inventory();

    override public bool PlayConstructedAnimation { get => false; }

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
        var eInput = (Electrical.Input)ElectricalNode;
        eInput.Production = powerProduction;
        eInput.MaxProduction = powerProduction;
        eInput.connectionRange = 6;  

        /*ElectricalRelay = new()
        {
            Parent = this 
        };
        ElectricalRelay.connectionRange = 6;*/

        GameManager.Instance.Lander = this;
    }

    public override void OnConstructed()
    { 
        /*ElectricalRelay.Constructed();*/
    }

    public override void OnDemolished()
    {
        base.OnDemolished();
        /*ElectricalRelay.Demolished();*/
    }

    void AddStartingResources()
    {
        inventory.maxItems = int.MaxValue-100; 

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