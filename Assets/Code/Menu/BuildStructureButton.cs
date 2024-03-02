using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildStructureButton : MonoBehaviour
{ 
    public StructureData structureData;

    [SerializeField] public Image image;

    public void ConfigureButton(StructureData structureData)
    {
        this.structureData = structureData;

        image = GetComponent<Image>();

        if(structureData.sprite != null)
        {
            image.sprite = structureData.sprite;
        } 
    } 

    public void ButtonPressed()
    {
        var gameManager = GameManager.Instance;

        gameManager.GetComponent<PlayerInputManager>().ChangeInputState(PlayerInputManager.InputState.Default);

        gameManager.constructionManager.StartPlacingGhosts(structureData);

        gameManager.GetComponent<PlayerInputManager>().ChangeInputState(PlayerInputManager.InputState.Construction);
    }
}
