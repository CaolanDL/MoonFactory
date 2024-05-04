using ExtensionMethods;
using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class ChargingPad : Structure
{
    public static List<ChargingPad> Pool = new();
    public float powerSupply = 0f; 
    public bool inUse = false; 

    public override void OnInitialise()
    {
        base.OnInitialise();

        ElectricalNode = new Electrical.Sink();
        Pool.Add(this);

        Debug.Log("Spawned Pad");

        Debug.Log(ElectricalNode.GetType().ToString());
    } 

    public override void OnDemolished()
    {
        base.OnDemolished();
        
        Pool.Remove(this);
    }

    public override void OnTick()
    {
        base.OnTick();

        if(ElectricalNode.Network != null)
        {
            powerSupply = ElectricalNode.Network.ClampedPowerRatio / 1000;
        }
        else
        {
            powerSupply = 0f;
        }
    }
} 