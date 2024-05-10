public class GenericMachine : Machine
{
    public override void OnInitialise()
    {
        base.OnInitialise();

        isCrafter = true;

        OutputInventories[0].maxItems = 32;
        OutputInventories[0].maxTypes = 1;

        InputInventories[0].maxItems = 32;
        InputInventories[0].maxTypes = 1;
    }

    public override void OnTick()
    {
        base.OnTick();

        TryBeginCrafting();

        TickCrafting();

        TryOutputAnything(0);
    }
}