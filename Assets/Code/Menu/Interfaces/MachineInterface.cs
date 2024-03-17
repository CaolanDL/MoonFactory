using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MachineInterface : ModularInterface
{
    private Machine machine;

    private StructureData structureData;

    public void Init(Machine machine, Vector3 screenPosition)
    {
        this.machine = machine;

        structureData = machine.structureData;

        SetDetails(structureData.sprite, structureData.screenname, structureData.description);

        transform.position = screenPosition;
    }
}
