using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildStructureButton : MonoBehaviour
{ 
    public StructureData StructureData;

    public void ButtonPressed()
    {
        var gameManager = GameManager.Instance;

        gameManager.GetComponent<PlayerInputManager>().ChangeInputState(PlayerInputManager.InputState.Default);

        gameManager.constructionManager.StartPlacingGhosts(StructureData);

        gameManager.GetComponent<PlayerInputManager>().ChangeInputState(PlayerInputManager.InputState.Construction);
    }
}
