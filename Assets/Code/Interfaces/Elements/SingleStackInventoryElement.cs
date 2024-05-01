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
        maxItems.SetText("/" + inventory.maxItems.ToString());
        if (resourceStack == null)
        {
            image.sprite = MenuData.Instance.emptySprite;
            counter.SetText("0"); 
            return;
        } 
        image.sprite = resourceStack.resource.sprite;
        counter.SetText(resourceStack.quantity.ToString()); 
    } 
}
