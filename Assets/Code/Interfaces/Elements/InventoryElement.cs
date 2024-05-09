using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryElement : MonoBehaviour
{
    public Inventory linkedInventory; 
    List<ResourceIcon> iconList = new();
    [SerializeField] GameObject IconPrefab;

    public void UpdateDetails()
    {
        foreach (var icon in iconList)
        {
            var quantity = linkedInventory.GetQuantityOf(icon.resource);

            if (linkedInventory.GetQuantityOf(icon.resource) > 0)
            {
                icon.SetCount(quantity);
            }
            else
            {
                Destroy(icon); 
            }
        }

        foreach(var stack in linkedInventory.stacks)
        { 
            if(!iconList.Exists(x => x.resource == stack.resource))
            {
                var newIcon = Instantiate(IconPrefab, transform);
                var newIconComponent = newIcon.GetComponent<ResourceIcon>();
                iconList.Add(newIconComponent);
                newIconComponent.SetDetails(stack.resource);
                newIconComponent.SetCount(stack.quantity);
            }
        }
    }
}
