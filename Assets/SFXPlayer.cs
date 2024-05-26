using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    [SerializeField] AudioClip AudioClip;

    public void PlaySound()
    {
        AudioManager.Instance?.PlaySound(AudioClip);
    }
}
