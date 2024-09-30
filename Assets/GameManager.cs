using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Dependencies")]
    private GameObject PlayerPrefab;
    
    [SerializeField] [FoldoutGroup("Dependencies")]
    private GameObject EnemyPrefab;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private Transform EntitiesContent;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private TextMeshProUGUI ScoreText;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private TextMeshProUGUI TimeText;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private MenuGroup ResultsMenuGroup;

    [SerializeField] [FoldoutGroup("Settings")]
    private float StartingTime;

    [SerializeField] [FoldoutGroup("Settings")]
    private int EnemySpawnsPerRound;

    [SerializeField] [FoldoutGroup("Events")]
    private UnityEvent OnStartGame = new();
    
    [SerializeField] [FoldoutGroup("Events")]
    private UnityEvent OnEndGame = new();

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private float RemainingTime;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private int SessionScore;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private bool GameActive;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private float TileSize;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Entity PlayerEntity;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private List<Entity> SpawnedEntities = new();

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private List<GameTile> SpawnedGameTiles = new();

    public static GameManager Singleton;

    private void Awake() {
        Singleton = this;
    }

    public static void StartGame(List<GameTile> tiles, float size) {
        var availableTiles = new List<GameTile>(tiles);
        Singleton.SpawnedGameTiles = tiles;
        Singleton.TileSize = size;
        Singleton.StartGame();
        Singleton.SpawnPlayer(availableTiles[0]);
        availableTiles.Remove(availableTiles[0]);
        Singleton.SpawnEnemies(availableTiles);
    }

    private void SpawnPlayer(GameTile tile) {
        var playerSpawnTile = tile;
        Entity player = Pooler.Spawn(PlayerPrefab, EntitiesContent).GetComponent<Entity>();
        PlayerEntity = player;
        player.SetupEntity(playerSpawnTile, TileSize);
        Singleton.SpawnedEntities.Add(player);
    }

    private void SpawnEnemies(List<GameTile> availableTiles) {
        if (!GameActive) return;
        for (int i = 0; i < EnemySpawnsPerRound; i++) {
            int r = Random.Range(0, availableTiles.Count);
            var spawnTile = availableTiles[r];
            availableTiles.Remove(availableTiles[r]);
            Entity enemy = Pooler.Spawn(EnemyPrefab, EntitiesContent).GetComponent<Entity>();
            enemy.SetupEntity(spawnTile, TileSize);
            Singleton.SpawnedEntities.Add(enemy);
        }
    }

    public static void CapturePiece(Entity capturedEntity) {
        if (!Singleton.GameActive) return;
        Singleton.SpawnedEntities.Remove(capturedEntity);
        Singleton.RemainingTime += 3f;
        Singleton.SessionScore++;
        Singleton.ScoreText.text = Singleton.SessionScore.ToString();
        if (Singleton.SpawnedEntities.Count == 1) {
            var availableTiles = new List<GameTile>(Singleton.SpawnedGameTiles);
            availableTiles.Remove(Singleton.PlayerEntity.CurrentTile);
            Singleton.SpawnEnemies(availableTiles);
        }
    }

    private void Update() {
        if (!GameActive) return;
        if (RemainingTime > 0) {
            RemainingTime -= Time.deltaTime;
            TimeText.text = RemainingTime.ToString("0.00");
        }
        else EndGame();
    }

    private void StartGame() {
        SessionScore = 0;
        ScoreText.text = SessionScore.ToString();
        RemainingTime = StartingTime;
        GameActive = true;
        OnStartGame.Invoke();
    }

    private void EndGame() {
        GameActive = false;
        ResetGame();
        ClearMoves();
        OnEndGame.Invoke();
    }

    public static void ClearMoves() {
        foreach (var tile in Singleton.SpawnedGameTiles) {
            tile.SetMoveValidity(null);
        }
    }

    public static void GetValidMoveTiles(Entity currentEntity) {
        if (!Singleton.GameActive) return;
        ClearMoves();
        var currentTile = currentEntity.CurrentTile;
        foreach (var move in currentEntity.ValidMoves) {
            var moveToPosition = currentTile.TilePosition + move;
            var possibleTile = Singleton.SpawnedGameTiles.FirstOrDefault(o => o.TilePosition == moveToPosition);
            possibleTile?.SetMoveValidity(currentEntity);
        }
    }

    public static void ResetGame() {
        foreach (var entity in Singleton.SpawnedEntities) {
            entity.JumpOffBoard();
        }

        foreach (var tile in Singleton.SpawnedGameTiles) {
            tile.SetOccupyingEntity(null);
        }
        Singleton.SpawnedEntities.Clear();
    }
}