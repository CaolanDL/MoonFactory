using Logistics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoverInterface : StaticInterface
{
    public Rover rover;

    public override void Init(Entity entity, Vector3 screenPosition)
    {
        this.rover = (Rover)entity;  

        transform.position = screenPosition; 
    }
}
