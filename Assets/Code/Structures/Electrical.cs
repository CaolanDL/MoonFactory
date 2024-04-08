public class PowerPylon : Structure
{  
    public override void OnConstructed()
    {
        ElectricalNode = new Electrical.Relay();
    }
}

public class SolarPanel : Structure
{
    public override void OnConstructed()
    {
        ElectricalNode = new Electrical.Input();
    }
}