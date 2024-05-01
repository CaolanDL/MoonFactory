using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "MoonFactory/Singletons/Audio Data")]
public class AudioData : ScriptableObject
{




    #region Singleton Instance
    public AudioData Instance { get; private set; }

    private void OnEnable() { MakeSingleton(); }

    private void OnValidate() { MakeSingleton(); }

    public void MakeSingleton()
    {
        if (Instance != null && Instance != this) Destroy(this); 
        else Instance = this; 
    }
    #endregion
}
