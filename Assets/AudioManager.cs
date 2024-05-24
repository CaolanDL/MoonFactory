using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] List<AudioClip> MusicTracks;
    int currentTrackIndex = 0;

    AudioSource AudioSource;

    private void Awake()
    {
        AudioSource = GetComponent<AudioSource>();
    }

    public void StartMusic()
    {
        AudioSource.Play();
    }

    public void StopMusic()
    {

    }
}
