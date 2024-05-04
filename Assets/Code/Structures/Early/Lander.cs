using System.Collections;
using UnityEngine;


public class Lander : Structure
{
    public float powerProduction = 50f;

    public Electrical.Relay ElectricalRelay;

    public override bool CanDemolish()
    {
        return false;
    } 
    
    public override void OnInitialise()
    {
        base.OnInitialise();

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
}