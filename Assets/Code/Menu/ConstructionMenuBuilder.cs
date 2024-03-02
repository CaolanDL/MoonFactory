using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConstructionMenuBuilder : MonoBehaviour
{
    [SerializeField] GridLayoutGroup gridLayout;

    [SerializeField] GameObject buildStructureButton;

    List<BuildStructureButton> buttons = new();

    private void Awake()
    {
        gridLayout = GetComponentInChildren<GridLayoutGroup>();

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
            var newButton = Instantiate(buildStructureButton, gridLayout.transform);

            var newBuildStructureButton = newButton.GetComponent<BuildStructureButton>();

            newBuildStructureButton.ConfigureButton(structureData);

            buttons.Add(newBuildStructureButton);
        }
    }
}
