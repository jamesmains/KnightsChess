using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
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
    private float SpawnDelay = 0.25f;

    [SerializeField] [FoldoutGroup("Settings")]
    private int FieldSize = 16;

    [SerializeField] [FoldoutGroup("Settings")]
    private Ease ShowEase;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private Ease HideEase;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private float ModifiedTileSize;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private List<RectTransform> SpawnedPlaceholderTiles;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private List<RectTransform> SpawnedGameTiles;

    private void Start() {
        ModifiedTileSize = ParentCanvas.sizeDelta.x / 4;
        Grid.cellSize = new Vector2(ModifiedTileSize,ModifiedTileSize);
        CreateBoard();
    }

    private void ClearTiles() {
        foreach (var placeholderTile in SpawnedPlaceholderTiles) {
            placeholderTile.gameObject.SetActive(false);
        }
        foreach (var gameTile in SpawnedGameTiles) {
            gameTile.gameObject.SetActive(false);
        }
    }

    [Button]
    private void CreateBoard() {
        StartCoroutine(SpawnTiles());

        IEnumerator SpawnTiles() {
            SpawnPlaceholderTiles();
            yield return new WaitForEndOfFrame();
            // for (int i = 0; i < FieldSize; i++) {
            //     SpawnGameTile(i);
            //     yield return new WaitForSeconds(SpawnDelay);
            // }
        }
    }

    [Button]
    private void OpenBoard() {
        StartCoroutine(ShowTiles());
        IEnumerator ShowTiles()
        {
            foreach (var placeholderTile in SpawnedPlaceholderTiles) {
                placeholderTile.gameObject.SetActive(true);
            }
            yield return new WaitForEndOfFrame();
            for (int i = 0; i < FieldSize; i++) {
                SpawnGameTile(i);
                yield return new WaitForSeconds(SpawnDelay);
            }
        }
    }
    
    [Button]
    private void CloseBoard() {
        StartCoroutine(HideTiles());

        IEnumerator HideTiles() {
            foreach (var placeholderTile in SpawnedPlaceholderTiles) {
                placeholderTile.gameObject.SetActive(false);
            }
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            // for (int i = FieldSize-1; i > -1; i--) {
            //     DespawnGameTile(i);
            //     yield return new WaitForSeconds(SpawnDelay);
            // }
            for (int i = 0; i < FieldSize; i++) {
                DespawnGameTile(i);
                yield return new WaitForSeconds(SpawnDelay);
            }
        }
    }

    private void SpawnPlaceholderTiles() {
        if (SpawnedPlaceholderTiles.Count > 0) return;
        for (int i = 0; i < FieldSize; i++) {
            var placeholderTile = Pooler.Spawn(PlaceholderTilePrefab, PlaceholderTileContent);
            placeholderTile.GetComponent<RectTransform>().sizeDelta =
                new Vector2(ModifiedTileSize, ModifiedTileSize);
            SpawnedPlaceholderTiles.Add(placeholderTile.GetComponent<RectTransform>());
        }
    }

    private void SpawnGameTile(int i) {
        bool create = i >= SpawnedGameTiles.Count;
        var gameTile = create ? Pooler.Spawn(FieldTilePrefab, GameTileContent) : SpawnedGameTiles[i].gameObject;
        var pos = new Vector3 {
            x = 0,
            y = -ParentCanvas.sizeDelta.y
        };
        var gameTileRect = create ? gameTile.GetComponent<RectTransform>() : SpawnedGameTiles[i];
        gameTileRect.anchoredPosition = pos;
        if(create)
            SpawnedGameTiles.Add(gameTileRect);
        gameTileRect.sizeDelta = new Vector2(ModifiedTileSize,ModifiedTileSize);
        gameTileRect.DOAnchorPos(SpawnedPlaceholderTiles[i].transform.localPosition, 1f).SetEase(ShowEase);
    }

    private void DespawnGameTile(int i) {
        var pos = new Vector3 {
            x = 0,
            y = -ParentCanvas.sizeDelta.y
        };
        var gameTileRect = SpawnedGameTiles[i];
        gameTileRect.DOAnchorPos(pos, 1f).SetEase(HideEase);
    }
}