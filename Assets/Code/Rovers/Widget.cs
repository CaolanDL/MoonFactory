﻿using UnityEngine;

public class Widget : Rover
{
    static float _MoveSpeed = 2.4f;
    public override float MoveSpeed => _MoveSpeed;

    static float _TurnSpeed = 5f;
    public override float TurnSpeed => _TurnSpeed;


    public Widget()
    {
        Module = RoverModule.Widget; 
    }

    public override void OnTick()
    {
        base.OnTick();

        powerLevel = Mathf.Clamp01(powerLevel + 0.001f);
    }
}