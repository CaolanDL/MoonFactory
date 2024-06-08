using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField] AudioMixer Mixer;
    [SerializeField] AudioMixerGroup MusicMixerGroup;
    [SerializeField] AudioMixerGroup UIMixerGroup;
    [SerializeField] AudioMixerGroup SFXMixerGroup;

    [SerializeField] AudioClip MainMenuMusic;

    [SerializeField] List<AudioClip> MusicTracks;
    int currentTrackIndex = 0;

    [SerializeField] AudioSource MusicSource;
    [SerializeField] AudioSource SFXSource;
      
    private void Awake()
    {
        MusicSource = GetComponent<AudioSource>();

        Mixer.GetFloat("MusicVolume", out MusicVolume);
        Mixer.GetFloat("WorldVolume", out WorldVolume);
        Mixer.GetFloat("UIVolume", out UIVolume);
    }

    public void PlayMainMenuMusic()
    {
        MusicSource.clip = MainMenuMusic;
        MusicSource.Play();
    }

    float MusicVolume;
    float WorldVolume;
    float UIVolume;

    public void EnableMusic(bool enable)
    {
        if(enable)
        {
            Mixer.SetFloat("MusicVolume", MusicVolume);
        }
        else
        {
            Mixer.SetFloat("MusicVolume", -80f);
        }
    } 

    public void EnableSFX(bool enable)
    {
        if(enable)
        {
            Mixer.SetFloat("WorldVolume", WorldVolume);
            Mixer.SetFloat("UIVolume", UIVolume);
        }
        else
        {
            Mixer.SetFloat("WorldVolume", -80f);
            Mixer.SetFloat("UIVolume", -80f);
        }
    } 

    public void StartMusic() { MusicSource.clip = MusicTracks[currentTrackIndex]; MusicSource.Play(); }
    public void StopMusic() => MusicSource.Stop();  

    public void PlaySound(AudioClip audioClip) => SFXSource.PlayOneShot(audioClip);
    public void PlaySound(AudioClip audioClip, float volume) => SFXSource.PlayOneShot(audioClip, volume);
}
