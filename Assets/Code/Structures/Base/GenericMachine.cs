public class GenericMachine : Machine
{
    public override void OnInitialise()
    {
        base.OnInitialise();

        isCrafter = true;

        foreach(var inv in OutputInventories)
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