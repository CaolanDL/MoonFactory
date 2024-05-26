using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioClip MainMenuMusic;

    [SerializeField] List<AudioClip> MusicTracks;
    int currentTrackIndex = 0;

    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;
      
    private void Awake()
    {
        MusicSource = GetComponent<AudioSource>(); 
    }

    public void PlayMainMenuMusic()
    {
        MusicSource.clip = MainMenuMusic;
        MusicSource.Play();
    } 

    public void StartMusic() { MusicSource.clip = MusicTracks[currentTrackIndex]; MusicSource.Play(); }
    public void StopMusic() => MusicSource.Stop();  

    public void PlaySound(AudioClip audioClip) => SFXSource.PlayOneShot(audioClip);
    public void PlaySound(AudioClip audioClip, float volume) => SFXSource.PlayOneShot(audioClip, volume);
}
