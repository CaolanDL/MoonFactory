﻿
using ExtensionMethods;

public class Crusher : Machine
{
    public override void OnInitialise()
    {
        base.OnInitialise();

        isCrafter = true;

        OutputInventories[0].maxItems = 10; 
        OutputInventories[0].maxTypes = 1;

        InputInventories[0].maxItems = 5; 
        InputInventories[0].maxTypes = 1;
    }

    public override void OnConstructed()
    {
        base.OnConstructed();

        TutorialProxy.Action.Invoke(TutorialEvent.CrusherBuilt);
    }

    public override void OnFrameUpdate()
    {
        base.OnFrameUpdate();

        if (TutorialProxy.IsActive)
        {
            TutorialProxy.SetPopupPosition(GameManager.Instance.CameraController.activeMainCamera.WorldToScreenPoint(DisplayObject.transform.position), TutorialTag.CrusherPosition);
        }
    }

    public override void OnTick()
    {
        base.OnTick();

        TryBeginCrafting(); 

        TickCrafting();

        TryOutputAnything(0);
    }

    public override void OnBeginCrafting()
    {
        DisplayObject.SetLoopingAnimation("Crushing");
        DisplayObject.PlayParticleEffect("CrushingParticles");
    }

    public override void OnStopCrafting() 
    {
        DisplayObject.SetLoopingAnimation("Idle");
        DisplayObject.StopParticleEffect("CrushingParticles");
    }
}

public class MagneticSeperator : Machine
{
    public override void OnInitialise()
    {
        base.OnInitialise();

        isCrafter = true;

        OutputInventories[0].maxItems = 10; 
        OutputInventories[0].maxTypes = 1;

        InputInventories[0].maxItems = 24; 
        InputInventories[0].maxTypes = 1;
    }

    public override void OnTick()
    {
        base.OnTick();

        TryBeginCrafting();

        TickCrafting();

        TryOutputAnything(0);
    }

    public override void OnBeginCrafting()
    { 
        DisplayObject.PlayParticleEffect("BeltParticles");
    }

    public override void OnStopCrafting()
    { 
        DisplayObject.StopParticleEffect("BeltParticles");
    }
}


public class ElectrostaticSeperator : Machine
{
    public override void OnInitialise()
    { 
        base.OnInitialise();

        ElectricalNode.Parent = this;

        isCrafter = true;

        foreach (var inv in OutputInventories)
        {
            inv.maxItems = 32;
            inv.maxTypes = 1;
        }
        foreach (var inv in InputInventories)
        {
            inv.maxItems = 32;
            inv.maxTypes = 1;
        }
    }

    public override void OnTick()
    {
        base.OnTick();

        TryBeginCrafting();

        TickCrafting();

        TryOutputAnything(0);
    }
}