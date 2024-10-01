using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class UsernameManager : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Settings")]
    private List<string> UsernameAdjectives = new();
    
    [SerializeField] [FoldoutGroup("Settings")]
    private List<string> UsernameNouns = new();

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public string CurrentUsername;

    public static UsernameManager Singleton;

    private void Awake() {
        Singleton = this;
        CurrentUsername = PlayerPrefs.GetString("Username",GetRandomUsername());
    }

    public static string GetRandomUsername() {
        int rAdjective = Random.Range(0, Singleton.UsernameAdjectives.Count);
        int rNouns = Random.Range(0, Singleton.UsernameNouns.Count);
        return $"{Singleton.UsernameAdjectives[rAdjective]} {Singleton.UsernameNouns[rNouns]}";
    }

    public static void SetUsername(string username) {
        Singleton.CurrentUsername = username;
        PlayerPrefs.SetString("Username",Singleton.CurrentUsername);
    }
}
