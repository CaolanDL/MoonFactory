using UnityEngine;
using UnityEngine.PlayerLoop;

public class PowerPylon : Structure
{
    public override void OnInitialise()
    {
        base.OnInitialise();
        ElectricalNode = new Electrical.Relay();
    } 

    public override void OnClicked(Vector3 mousePosition)
    {
        OpenInterfaceOnHUD(MenuData.Instance.RelayInterface, mousePosition);
    }
}

public class SolarPanel : Structure
{
    Electrical.Input ElectricalInput;

    public override void OnInitialise()
    {
        base.OnInitialise();
        ElectricalNode = new Electrical.Input();
        ElectricalInput = (Electrical.Input)ElectricalNode;
        ElectricalInput.Parent = this;
        ElectricalInput.MaxProduction = 100;
    }

    public override void OnConstructed()
    {
        base.OnConstructed();

        //ElectricalInput.Constructed();
    }

    public override void OnTick()
    {
        ElectricalInput.Production = ElectricalInput.MaxProduction * Mathf.PerlinNoise1D(Time.fixedTime);
    }

    public override void OnClicked(Vector3 mousePosition)
    {
        OpenInterfaceOnHUD(MenuData.Instance.RelayInterface, mousePosition);
    }
}