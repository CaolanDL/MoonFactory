using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuildStructureButton : MonoBehaviour
{ 
    public StructureData structureData;

    [SerializeField] public Image image;

    [SerializeField] public TextMeshProUGUI nameText;

    public void ConfigureButton(StructureData structureData)
    {
        this.structureData = structureData;

        image = GetComponent<Image>();

        nameText = GetComponentInChildren<TextMeshProUGUI>();

        if (structureData.sprite != null)
        {
            image.sprite = structureData.sprite;
        }
         
        nameText.SetText(structureData.screenname); 
    } 

    public void ButtonPressed()
    {
        var gameManager = GameManager.Instance;

        gameManager.GetComponent<PlayerInputManager>().ChangeInputState(PlayerInputManager.InputState.Default);

        gameManager.ConstructionManager.StartPlacingGhosts(structureData);

        gameManager.GetComponent<PlayerInputManager>().ChangeInputState(PlayerInputManager.InputState.Construction);
    }
}
