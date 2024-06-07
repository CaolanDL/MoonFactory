using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Loading;
using UnityEngine;
using UnityEngine.UI;

public class ResourceInterface : MonoBehaviour
{
    [SerializeField] TMP_Text resourceName;
    [SerializeField] TMP_Text resourceType;

    [SerializeField] Transform requirementsLayout;
    [SerializeField] ResourceIcon resourceIcon;
    [SerializeField] TMP_Text craftTime;

    [SerializeField] Image scienceIcon;
    [SerializeField] TMP_Text scienceValue;

    [SerializeField] Image structureImage;
    [SerializeField] TMP_Text structureName;

    [SerializeField] Transform craftsIntoLayout;

    [SerializeField] GameObject RequirementResourceIcon;
    [SerializeField] GameObject CraftsIntoResourceIcon;

    [SerializeField] Transform buildsIntoLayout;
    [SerializeField] GameObject structureIcon;


    ResourceData Resource;

    public void SetResource(ResourceData resource)
    {
        Resource = resource;
        SetDetails();
    }

    void SetDetails()
    {
        resourceName.text = Resource.name;
        resourceType.text = Resource.resourceCategory.ToString() + " Resource";
        resourceIcon.SetDetails(Resource);
        resourceIcon.SetCount(Resource.quantityCrafted);

        foreach (var rq in Resource.requiredResources)
        {
            var icon = Instantiate(RequirementResourceIcon, requirementsLayout);
            var iconComponent = icon.GetComponent<ResourceIcon>();
            iconComponent.SetDetails(rq.resource);
            iconComponent.SetCount(rq.quantity);
        }

        craftTime.text = (Resource.timeToCraft / 50f).ToString();

        if (GameManager.Instance.ScienceManager.IsResearchedIn(Resource, ScienceManager.Researcher.Analyser))
        {
            scienceValue.color = new Color(24, 24, 24);
            scienceIcon.color = new Color(24, 24, 24);
        }
        scienceValue.text = Resource.ResearchValue.ToString();

        structureImage.sprite = Resource.craftedIn.sprite;
        structureName.text = Resource.craftedIn.name;

        foreach (var resource in GlobalData.Instance.Resources)
        {
            if (resource.requiredResources.Exists(r => r.resource == Resource))
            {
                var icon = Instantiate(CraftsIntoResourceIcon, craftsIntoLayout);
                var iconComponent = icon.GetComponent<ResourceIcon>();
                iconComponent.SetDetails(resource);
            }
        }

        foreach (var structure in GlobalData.Instance.Structures)
        {
            if (structure.name == "Lander") { continue; }
            if (structure.requiredResources.Exists(r => r.resource == Resource))
            {
                var icon = Instantiate(structureIcon, buildsIntoLayout);
                var iconComponent = icon.GetComponent<StructureIcon>();
                iconComponent.SetStructure(structure);
            }
        }
    }

    public void Close()
    {
        Destroy(gameObject);
    }
}
