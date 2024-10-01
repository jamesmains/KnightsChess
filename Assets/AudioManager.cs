using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Dependencies")]
    private AudioSource SfxSource;
    
    [SerializeField] [FoldoutGroup("Dependencies")]
    private AudioSource BgmSource;
    
    [SerializeField] [FoldoutGroup("Dependencies")]
    private AudioMixerGroup SfxGroup;
    
    [SerializeField] [FoldoutGroup("Dependencies")]
    private AudioMixerGroup BgmGroup;

    [SerializeField] [FoldoutGroup("Settings")]
    private AudioClip UI_SfxClip; // If game expands, this will need to be reworked
    
    [SerializeField] [FoldoutGroup("Settings")]
    private AudioClip Game_SfxClip; // If game expands, this will need to be reworked

    public static AudioManager Singleton;
    
    private void Awake() {
        Singleton = this;
        BgmSource.Play();
    }

    public static void PlayUISfxClip() {
        Singleton.SfxSource.PlayOneShot(Singleton.UI_SfxClip);
    }

    public static void PlayGameSfxClip() {
        Singleton.SfxSource.PlayOneShot(Singleton.Game_SfxClip);
    }

    public static void SetSfxGroupVolume(float volume) {
        Singleton.SfxGroup.audioMixer.SetFloat("Volume_Sfx", volume);
    }

    public static void SetBgmGroupVolume(float volume) {
        Singleton.BgmGroup.audioMixer.SetFloat("Volume_Bgm", volume);
        if (volume >= 0) {
            Singleton.BgmSource.Play();
        }
        else Singleton.BgmSource.Stop();
    }
    
}
