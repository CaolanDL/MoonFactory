using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionMenuBuilder : MonoBehaviour
{  
    [SerializeField] GridLayoutGroup LogisticsBox;
    [SerializeField] GridLayoutGroup ProcessingBox;
    [SerializeField] GridLayoutGroup ManufacturingBox;
    [SerializeField] GridLayoutGroup PowerBox;
    [SerializeField] GridLayoutGroup ScienceBox; 

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

        foreach(var structureData in GlobalData.Instance.structures)
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

            var newButton = Instantiate(buildStructureButton, parent);

            var newBuildStructureButton = newButton.GetComponent<BuildStructureButton>();

            newBuildStructureButton.ConfigureButton(structureData);

            buttons.Add(newBuildStructureButton);
        } 
    } 
}
