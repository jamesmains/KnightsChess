using System;
using System.Collections.Generic;
using Dan.Main;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class Leaderboard : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Dependencies")]
    private List<LeaderboardListing> Listings = new();

    public string BoardKey = "ed81c1b1e6ceae4371d1bac7f8b39afd9148c02963f1e0ddf94804b57e5036ad";

    public static Leaderboard Singleton;
    
    private void Awake() {
        Singleton = this;
    }

    public static void SetNewLeaderboardScore(int score) {
        LeaderboardCreator.UploadNewEntry(Singleton.BoardKey, UsernameManager.Singleton.CurrentUsername ,score,(
            (msg) => {
                Singleton.UpdateLeaderboard();
            }));
    }

    public void UpdateLeaderboard() {
        foreach (var t in Listings) {
            t.UsernameText.text = "Loading";
            t.ScoreText.text = "Loading";
        }

        LeaderboardCreator.GetLeaderboard(BoardKey, ((msg) => {
            for (int i = 0; i < Listings.Count; i++) {
                if (i >= msg.Length) {
                    Listings[i].UsernameText.text = "No Score";
                    Listings[i].ScoreText.text = "";    
                    continue;
                }
                print($"Msg count: {msg.Length}, i: {i}, i > length: {i >= msg.Length}, i > length -1: {i >= msg.Length - 1}");
                Listings[i].UsernameText.text = msg[i].Username;
                Listings[i].ScoreText.text = msg[i].Score.ToString();
            }
        }));
    }
}

[Serializable]
public class LeaderboardListing {
    public TextMeshProUGUI UsernameText;
    public TextMeshProUGUI ScoreText;
}
