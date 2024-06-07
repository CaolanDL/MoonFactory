using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleStackInventoryElement : MonoBehaviour
{
    [SerializeField] ResourceIcon icon;

    public Image image;
    public TextMeshProUGUI counter;
    public TextMeshProUGUI maxItems;

    public Inventory inventory;
    public ResourceStack resourceStack;

    private void Start()
    {
        inventory.InventoryUpdated += UpdateDisplay;
    }

    public void UpdateDisplay()
    {
        if (icon == null) { return; }
        if (icon.gameObject == null) { return; }

        maxItems.SetText(inventory.maxItems.ToString());
        if (resourceStack == null)
        {
            icon.gameObject.SetActive(false);
            image.sprite = MenuData.Instance.emptySprite;
            counter.SetText("0"); 
            return;
        } 
        icon.gameObject.SetActive(true);
        icon.SetDetails(resourceStack.resource);
        icon.SetCount(resourceStack.quantity); 
    } 
}
