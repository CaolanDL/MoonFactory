using ModularInterfaces;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static ScienceManager;

public class LanderInterface : StaticInterface
{
    Lander lander;

    [SerializeField] InventoryElement inventory;

    public override void Init(Entity entity, Vector3 screenPosition)
    {
        base.Init(entity, screenPosition);

        lander = (Lander)entity; 

        inventory.linkedInventory = lander.inventory;   
    }

    private void Update()
    {
        UpdateUI();
    } 

    public override void UpdateUI()
    {
        base.UpdateUI();

        inventory.UpdateDetails();
    } 
}
