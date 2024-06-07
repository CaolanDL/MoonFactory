using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ResourceToolTip : MonoBehaviour
{
    ResourceData resource;

    [SerializeField] TMP_Text tooltipName;
    [SerializeField] Image tooltipSprite;
    [SerializeField] TMP_Text quantityProduced;
    [SerializeField] TMP_Text machineName;
    [SerializeField] Image machineSprite;
    [SerializeField] GameObject RequirementsLayout;

    [SerializeField] GameObject CraftingRequirmentPrefab;
    List<GameObject> craftingRequirments = new();

    private void Awake()
    {
        transform.position = Input.mousePosition;
    }

    Vector3 velocity = Vector3.zero;
    private void Update()
    {
        transform.position = Vector3.SmoothDamp(transform.position, Input.mousePosition, ref velocity, 0.1f);
    }

    public void SetDetails(ResourceData resouce)
    { 
        this.resource = resouce;
         
        //tooltipSprite.sprite = resouce.sprite; 
        tooltipName.text = resource.name;

        /*quantityProduced.text = resource.quantityCrafted.ToString();*/
        machineName.text = resource.craftedIn.screenname;
        machineSprite.sprite = resource.craftedIn.sprite;

        foreach (var cr in craftingRequirments)
        {
            Destroy(cr.gameObject);
        }
        craftingRequirments.Clear();

        foreach (var cr in resouce.requiredResources)
        {
            var newCr = Instantiate(CraftingRequirmentPrefab, RequirementsLayout.transform);
            newCr.GetComponent<ResourceIcon>().SetCount(cr.quantity);
            newCr.GetComponent<ResourceIcon>().SetDetails(cr.resource);
            craftingRequirments.Add(newCr);
        }
    }
}
