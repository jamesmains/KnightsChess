using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public enum Group {
    Sfx,
    Bgm
}

public class AudioToggle : MonoBehaviour {
    [SerializeField] [BoxGroup("Dependencies")]
    private GameObject ToggleImg;
    
    [SerializeField] [BoxGroup("Settings")]
    private string AudioTarget;

    [SerializeField] [BoxGroup("Settings")]
    private Group AudioGroupTarget;

    [SerializeField] [BoxGroup("Status")] [ReadOnly]
    private float AudioVolume;

    private void Start() {
        AudioVolume = PlayerPrefs.GetFloat(AudioTarget, -100);
        SetVolume();
        SetToggleImg();
    }

    [Button]
    public void Toggle() {
        AudioVolume = AudioVolume >= 0 ? -100 : 0;
        PlayerPrefs.SetFloat(AudioTarget,AudioVolume);
        SetVolume();
        SetToggleImg();
    }

    private void SetVolume() {
        switch (AudioGroupTarget) {
            case Group.Sfx:
                AudioManager.SetSfxGroupVolume(AudioVolume);
                break;
            case Group.Bgm:
                AudioManager.SetBgmGroupVolume(AudioVolume);
                break;
        }
    }

    private void SetToggleImg() {
        print($"Volume: {AudioVolume}, Check: {AudioVolume >= 0}");
        ToggleImg.SetActive(AudioVolume >= 0);
    }
}
