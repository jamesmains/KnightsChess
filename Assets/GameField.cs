using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameField : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Dependencies")]
    private RectTransform ParentCanvas;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private GridLayoutGroup Grid;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private GameObject PlaceholderTilePrefab;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private GameObject FieldTilePrefab;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private Transform PlaceholderTileContent;

    [SerializeField] [FoldoutGroup("Dependencies")]
    private Transform GameTileContent;

    [SerializeField] [FoldoutGroup("Settings")]
    private Color PrimaryColor;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private Color SecondaryColor;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private float SpawnDelay = 0.25f;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private float EntitySpawnDelay = 0.75f;

    [SerializeField] [FoldoutGroup("Settings")]
    private int RowSize = 4;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private int ColumnSize = 4;

    [SerializeField] [FoldoutGroup("Settings")]
    private Ease ShowEase;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private Ease HideEase;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private float ModifiedTileSize;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private int CachedRowSize;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private int CachedColumnSize;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private List<RectTransform> SpawnedPlaceholderTiles;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private List<GameTile> SpawnedGameTiles;

    private Vector2 TilePosition;
    private int BoardSize()=>RowSize * ColumnSize;

    public static Vector2 CanvasSize;

    private void Start() {
        CreateBoard();
    }

    // Unsure if this will even be used
    private void ClearTiles() {
        foreach (var placeholderTile in SpawnedPlaceholderTiles) {
            placeholderTile.gameObject.SetActive(false);
        }

        SpawnedPlaceholderTiles.Clear();
        foreach (var gameTile in SpawnedGameTiles) {
            gameTile.gameObject.SetActive(false);
        }
        SpawnedGameTiles.Clear();
    }

    [Button]
    private void CreateBoard() {
        ClearTiles();
        CanvasSize = ParentCanvas.sizeDelta;
        ModifiedTileSize = ParentCanvas.sizeDelta.x / RowSize;
        Grid.cellSize = new Vector2(ModifiedTileSize,ModifiedTileSize);
        SpawnPlaceholderTiles();
        CachedRowSize = RowSize;
        CachedColumnSize = ColumnSize;
    }

    [Button]
    public void OpenBoard() {
        if (RowSize != CachedRowSize || ColumnSize != CachedColumnSize) {
            CreateBoard();
        }
        StartCoroutine(ShowTiles());
        IEnumerator ShowTiles()
        {
            foreach (var placeholderTile in SpawnedPlaceholderTiles) {
                placeholderTile.gameObject.SetActive(true);
            }
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < BoardSize(); i++) {
                SpawnGameTile(i);
                yield return new WaitForSeconds(SpawnDelay);
            }
            yield return new WaitForSeconds(EntitySpawnDelay);
            GameManager.StartGame(SpawnedGameTiles,ModifiedTileSize);
        }
    }
    
    [Button]
    public void CloseBoard() {
        StartCoroutine(HideTiles());

        IEnumerator HideTiles() {
            foreach (var placeholderTile in SpawnedPlaceholderTiles) {
                placeholderTile.gameObject.SetActive(false);
            }
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < SpawnedGameTiles.Count; i++) {
                DespawnGameTile(i);
                yield return new WaitForSeconds(SpawnDelay);
            }
        }
    }

    private void SpawnPlaceholderTiles() {
        for (int i = 0; i < BoardSize(); i++) {
            var placeholderTile = Pooler.Spawn(PlaceholderTilePrefab, PlaceholderTileContent);
            placeholderTile.GetComponent<RectTransform>().sizeDelta =
                new Vector2(ModifiedTileSize, ModifiedTileSize);
            SpawnedPlaceholderTiles.Add(placeholderTile.GetComponent<RectTransform>());
        }

        SpawnedPlaceholderTiles.Reverse();
    }

    private void SpawnGameTile(int i) {
        bool create = i >= SpawnedGameTiles.Count;
        
        var gameTile = create ? Pooler.Spawn(FieldTilePrefab, GameTileContent) : SpawnedGameTiles[i].gameObject;
        var pos = new Vector3 {
            x = 0,
            y = -ParentCanvas.sizeDelta.y
        };
        var tile = gameTile.GetComponent<GameTile>();
        if (create) {
            
            var divisibleRow = RowSize % 2 == 0;
            int index;
            if(!divisibleRow)
                index = (i / BoardSize()) % 2 == 0 ? i : i + 1;
            else index = (i / RowSize + ColumnSize) % 2 == 0 ? i : i + 1;
            tile.SetImageColor(index % 2 == 0 ? PrimaryColor : SecondaryColor);
            tile.SetTilePosition(TilePosition);
            TilePosition.x++;
            if (TilePosition.x >= RowSize) {
                TilePosition.x = 0;
                TilePosition.y++;
            }
        }
        tile.TileRect.anchoredPosition = pos;
        if(create)
            SpawnedGameTiles.Add(tile);
        tile.TileRect.sizeDelta = new Vector2(ModifiedTileSize,ModifiedTileSize);
        tile.TileRect.DOAnchorPos(SpawnedPlaceholderTiles[i].transform.localPosition, 1f).SetEase(ShowEase);
    }

    private void DespawnGameTile(int i) {
        var pos = new Vector3 {
            x = 0,
            y = -ParentCanvas.sizeDelta.y
        };
        var gameTileRect = SpawnedGameTiles[i];
        gameTileRect.TileRect.DOAnchorPos(pos, 1f).SetEase(HideEase);
    }
}