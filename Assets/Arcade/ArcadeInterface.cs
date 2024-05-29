using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeInterface : StaticInterface
{
    private void OnEnable()
    {
        SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
    }

    private void OnDisable()
    {
        SceneManager.UnloadSceneAsync(1);
    }
}
