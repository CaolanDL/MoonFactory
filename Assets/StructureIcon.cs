using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StructureIcon : MonoBehaviour
{
    StructureData structure;
    [SerializeField] Image sprite;
    GenericTooltipCreator tooltipCreator;

    public void SetStructure(StructureData structure)
    {
        this.structure = structure;
    }

    private void Start()
    {
        tooltipCreator = GetComponent<GenericTooltipCreator>();
        tooltipCreator.Header = structure.screenname;

        sprite.sprite = structure.sprite;
    }
}
