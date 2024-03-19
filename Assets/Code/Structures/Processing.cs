
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

    public override void OnTick()
    {
        base.OnTick();

        TryBeginCrafting(); 

        TickCrafting();

        TryOutputAnything(0);
    }

    public override void OnBeginCrafting()
    {
        displayObject.SetLoopingAnimation("Crushing");
        displayObject.PlayParticleEffect("CrushingParticles");
    }

    public override void OnStopCrafting() 
    {
        displayObject.SetLoopingAnimation("Idle");
        displayObject.StopParticleEffect("CrushingParticles");
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
        displayObject.PlayParticleEffect("BeltParticles");
    }

    public override void OnStopCrafting()
    { 
        displayObject.StopParticleEffect("BeltParticles");
    }
}
