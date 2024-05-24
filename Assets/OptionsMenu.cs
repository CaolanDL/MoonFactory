using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionsMenu : MonoBehaviour
{ 
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
}
