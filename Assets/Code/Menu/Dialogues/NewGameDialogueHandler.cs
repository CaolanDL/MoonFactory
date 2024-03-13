using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NewGameDialogueHandler : MonoBehaviour
{
    [SerializeField] TMP_InputField fileNameInput;

    private void Awake()
    {
        fileNameInput = GetComponentInChildren<TMP_InputField>();
    }

    public void CreateNewGame()
    {
        GameManager.Instance.CreateNewGame(fileNameInput.text);
        MainMenuManager.Instance.CloseMenu();
    }
}
