using Logistics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoverInterface : StaticInterface
{
    [SerializeField] Slider powerSlider;

    public Rover rover; 

    public override void Init(Entity entity, Vector3 screenPosition)
    {
        base.Init(entity, screenPosition);

        this.rover = (Rover)entity;  

        transform.position = screenPosition;

        AudioManager.Instance.PlaySound(AudioData.Instance.UI_WidgetInterfaceOpened);
    }

    private void FixedUpdate()
    {
        powerSlider.value = rover.powerLevel / Rover.maxPowerLevel;
    }
}
