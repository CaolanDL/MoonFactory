using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleStackInventoryElement : MonoBehaviour
{
    public Image image;
    public TextMeshProUGUI counter;
    public TextMeshProUGUI maxItems;

    public Inventory inventory;
    public ResourceStack resourceStack;

    public void UpdateDisplay()
    {
        if(resourceStack == null)
        {
            image.sprite = MenuData.Instance.emptySprite;
            counter.SetText($"<size=62%> Max {inventory.maxItems.ToString()}");
            return;
        }

        image.sprite = resourceStack.resource.sprite;
        counter.SetText($"{resourceStack.quantity.ToString()}<size=62%> / {inventory.maxItems.ToString()}"); 
    } 
}
