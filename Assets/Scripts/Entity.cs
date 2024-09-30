using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public class Entity : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Dependencies")]
    private RectTransform EntityRect;

    [SerializeField] [FoldoutGroup("Settings")]
    private int Team;

    [SerializeField] [FoldoutGroup("Settings")]
    private bool LightningMove;

    [SerializeField] [FoldoutGroup("Settings")]
    private Ease SpawnToTileEase;

    [SerializeField] [FoldoutGroup("Settings")]
    private Ease MoveToTileEase;

    [SerializeField] [FoldoutGroup("Settings")]
    private Ease MoveScaleEase;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private Ease JumpOffEase;

    [SerializeField] [FoldoutGroup("Settings")]
    private float MoveScaleSquishAmount;

    [SerializeField] [FoldoutGroup("Settings")]
    public List<Vector2> ValidMoves = new();

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Vector2 CachedSize;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public GameTile CurrentTile;

    private Vector2 SquishSize() => new Vector2(CachedSize.x, CachedSize.y * MoveScaleSquishAmount);

    public void SetupEntity(GameTile startingTile, float size) {
        size *= .75f;
        EntityRect.sizeDelta = new Vector2(size, size);
        CachedSize = EntityRect.sizeDelta;
        CurrentTile = startingTile;
        CurrentTile.SetOccupyingEntity(this);
        StartCoroutine(SpawnAnimation());

        IEnumerator SpawnAnimation() {
            EntityRect.anchoredPosition = new Vector2(CurrentTile.TileRect.anchoredPosition.x, 2000);
            var targetPos = CurrentTile.TileRect.anchoredPosition;
            targetPos.y -= EntityRect.sizeDelta.y / 2;
            yield return EntityRect.DOAnchorPos(targetPos, 0.5f)
                .SetEase(SpawnToTileEase).WaitForCompletion();
            yield return EntityRect.DOSizeDelta(SquishSize(), 0.05f).SetEase(MoveScaleEase).WaitForCompletion();

            yield return EntityRect.DOSizeDelta(CachedSize, 1f).SetEase(MoveScaleEase).WaitForCompletion();
            
        }
        if(Team == 0)
            GetValidMoveTiles();
    }

    public void MoveToTile(GameTile newTargetTile) {
        CurrentTile?.SetOccupyingEntity(null);
        CurrentTile = newTargetTile;
        GameManager.ClearMoves();
        StartCoroutine(MoveToAnimation());

        IEnumerator MoveToAnimation() {
            var targetPos = CurrentTile.TileRect.anchoredPosition;
            targetPos.y -= EntityRect.sizeDelta.y / 2;
            if (LightningMove) {
                EntityRect.anchoredPosition = targetPos;
            }
            else {
                yield return EntityRect.DOAnchorPos(targetPos, 0.5f).SetEase(MoveToTileEase)
                    .WaitForCompletion();
                
            }

            if (Team == 0) {
                CurrentTile.SetOccupyingEntity(this);
                GetValidMoveTiles();    
            }
            
        }
        EntityRect.DOSizeDelta(SquishSize(), 0.05f).SetEase(MoveScaleEase).WaitForCompletion();
        EntityRect.DOSizeDelta(CachedSize, 1f).SetEase(MoveScaleEase).WaitForCompletion();
    }

    public void JumpOffBoard() {
        StartCoroutine(JumpOffAnimation());
        IEnumerator JumpOffAnimation() {
            var targetPos = Vector2.zero;
            targetPos.x += EntityRect.anchoredPosition.x >= 0 ? targetPos.x += GameField.CanvasSize.x + EntityRect.sizeDelta.x : targetPos.x -= GameField.CanvasSize.x*1.5f - EntityRect.sizeDelta.x;
            EntityRect.DORotate(new Vector3(0, 0, 359.999f), .75f, RotateMode.FastBeyond360);
            yield return EntityRect.DOJumpAnchorPos(targetPos , 500,0,.75f).SetEase(JumpOffEase).WaitForCompletion();
            gameObject.SetActive(false);
        }
    }

    [Button]
    public void GetValidMoveTiles() {
        GameManager.GetValidMoveTiles(this);
    }
}