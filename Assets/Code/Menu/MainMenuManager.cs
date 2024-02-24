using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject NewGameDialogue;

    #region Singleton Instanciate
    public static MainMenuManager Instance { get; private set; }

    public void MakeSingleton()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }
    #endregion

    private void Awake()
    {
        MakeSingleton();
    }

    public void StartNewGamePressed()
    {
        Instantiate(NewGameDialogue, transform);
    }

    public void CloseMenu()
    {
        Destroy(gameObject);
    }
}
