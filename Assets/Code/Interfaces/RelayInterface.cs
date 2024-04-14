using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Electrical;
using UnityEngine.UI;
using TMPro;

public class RelayInterface : StaticInterface
{
    [SerializeField] TextMeshProUGUI MaxInput;
    [SerializeField] TextMeshProUGUI Input; 
    [SerializeField] TextMeshProUGUI MaxOutput;
    [SerializeField] TextMeshProUGUI Output; 
    [SerializeField] TextMeshProUGUI Ratio;  

    [SerializeField] Slider InputPowerMeter;
    [SerializeField] Slider OutputPowerMeter;
    [SerializeField] Slider PowerRatioMeter;

    Relay Relay;

    public override void Init(Entity entity, Vector3 screenPosition)
    {
        base.Init(entity, screenPosition);

        Relay = (Relay)((PowerPylon)entity).ElectricalNode;
    }

    private void FixedUpdate()
    {
        UpdateUI();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();

        if(Relay == null || Relay.Network == null) { WriteEmptyValues(); return; }

        WriteFromNetwork();
    }

    void WriteFromNetwork()
    {
        var network = Relay.Network;

        MaxInput.text = network.MaxInput.ToString();
        Input.text = network.TotalInput.ToString();
        MaxOutput.text = network.MaxConsumption.ToString();
        Output.text = network.TotalConsumption.ToString();
        Ratio.text = network.PowerRatio.ToString();

        InputPowerMeter.value = network.TotalInput / network.MaxInput;
        OutputPowerMeter.value = network.TotalConsumption / network.MaxConsumption;
        PowerRatioMeter.value = network.ClampedPowerRatio;
    }

    void WriteEmptyValues()
    {
        MaxInput.text = "";
        Input.text = "";
        MaxOutput.text = "";
        Output.text = "";
        Ratio.text = "NO NETWORK";
    }
}
