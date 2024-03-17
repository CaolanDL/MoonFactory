
public class Crusher : Machine
{
    public override void OnInitialise()
    {
        base.OnInitialise();

        isCrafter = true;

        OutputInventories[0].maxItems = 5;
        OutputInventories[0].maxWeight = 9999;
        OutputInventories[0].maxTypes = 1;

        InputInventories[0].maxItems = 5;
        InputInventories[0].maxWeight = 9999;
        InputInventories[0].maxTypes = 1;
    }

    public override void OnTick()
    {
        base.OnTick();

        TryBeginCrafting(); 

        TickCrafting();
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
