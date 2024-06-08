using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{
    [SerializeField] GameObject ExitDialogue;

    Vector3 startpos = Vector3.zero;

    private void Awake()
    {
        startpos = transform.position;
    }

    public void SetMusicActive(bool active)
    {
        if(active)
        {
            GameManager.Instance.AudioManager.StartMusic();
        }
        if (!active)
        {
            GameManager.Instance.AudioManager.StopMusic();
        }
    }

    public void SetQualityLevel(int level)
    {
        QualitySettings.SetQualityLevel(level);
    }

    public void OpenExitDialogue()
    {
        ExitDialogue.SetActive(true);   
    }

    public void ExitGame()
    {
        GameManager.Instance.ExitToMenu();
    }

    
    internal void ResetPosition()
    {
        transform.position = startpos;  
    }
}
