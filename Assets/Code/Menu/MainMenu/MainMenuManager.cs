using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject NewGameDialogue;
    [SerializeField] Canvas Canvas;

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

    private void OnDisable()
    {
        GameManager.Instance.DestroyMainMenuCamera();
    }

    private void Start()
    {
        GameManager.Instance.SpawnMainMenuCamera();
        Canvas.worldCamera = GameManager.Instance._mainMenuCamera; 

        Canvas.ForceUpdateCanvases();
    }

    public void StartNewGamePressed()
    {
        Instantiate(NewGameDialogue, Canvas.transform);
    }

    public void ExitGamePressed()
    {
        Application.Quit();
    }

    public void CloseMenu()
    {
        GameManager.Instance.DestroyMainMenuCamera();
        Destroy(gameObject);
    } 
}
