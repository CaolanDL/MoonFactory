using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionMenuBuilder : MonoBehaviour
{  
    [SerializeField] Transform LogisticsBox;
    [SerializeField] Transform ProcessingBox;
    [SerializeField] Transform ManufacturingBox;
    [SerializeField] Transform PowerBox;
    [SerializeField] Transform ScienceBox; 

    [SerializeField] GameObject buildStructureButton;

    List<BuildStructureButton> buttons = new();

    private void Awake()
    { 
        UpdateList();
    } 

    public void UpdateList()
    {
        foreach (var button in buttons)
        {
            if (button != null)
            {
                Destroy(button.gameObject);
            }
        }

        buttons.Clear();

        foreach(var structureData in GameManager.Instance.ScienceManager.unlocked_Structures)
        {
            Transform parent;

            switch (structureData.category)
            {
                case StructureCategory.Logistics:
                    parent = LogisticsBox.transform; break;
                case StructureCategory.Processing:
                    parent = ProcessingBox.transform; break;
                case StructureCategory.Manufacturing:
                    parent = ManufacturingBox.transform; break;
                case StructureCategory.Power:
                    parent = PowerBox.transform; break;
                case StructureCategory.Science:
                    parent = ScienceBox.transform; break;
                default:
                    parent = LogisticsBox.transform; break;
            }

            parent.parent.gameObject.SetActive(true);

            var newButton = Instantiate(buildStructureButton, parent);

            var newBuildStructureButton = newButton.GetComponent<BuildStructureButton>();

            newBuildStructureButton.ConfigureButton(structureData);

            buttons.Add(newBuildStructureButton);
        } 
    } 
}
