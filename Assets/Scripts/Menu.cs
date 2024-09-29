using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

public enum MenuState {
    Closed,
    Open
}

[RequireComponent(typeof(CanvasGroup))]
public class Menu : MonoBehaviour {
    [SerializeField] [FoldoutGroup("Settings")]
    private MenuState InitialState = MenuState.Open;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private Vector2 OpenPosition;

    [SerializeField] [FoldoutGroup("Settings")]
    private Vector2 ClosePosition;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private Ease EaseType = Ease.OutQuint;

    [SerializeField] [FoldoutGroup("Settings")]
    private float Speed = 0.3f;

    [SerializeField] [FoldoutGroup("Dependencies")] [ReadOnly]
    private CanvasGroup CanvasGroup;

    [SerializeField] [FoldoutGroup("Dependencies")] [ReadOnly]
    private RectTransform Rect;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    public MenuState State;
    
    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private Vector2 TargetPosition;
#if UNITY_EDITOR
    private void OnValidate() {
        if (TryGetComponent(out CanvasGroup cg)) {
            CanvasGroup = cg;
        }

        if (TryGetComponent(out RectTransform r)) {
            Rect = r;
        }
    }
#endif
    private void Awake() {
        if (CanvasGroup == null)CanvasGroup = GetComponent<CanvasGroup>();
        if (Rect == null) Rect = GetComponent<RectTransform>();
        if(InitialState == MenuState.Closed)
            Close();
        else Open();
    }
    [Button]
    public void Open() {
        TargetPosition = OpenPosition;
#if UNITY_EDITOR
        if (!Application.isPlaying) {
            Rect.anchoredPosition = TargetPosition;
            return;
        }
#endif
        StartCoroutine(DoOpen());
        IEnumerator DoOpen() {
            Tween menuTween = Rect.DOAnchorPos(TargetPosition, Speed)
                .SetEase(EaseType);
            yield return menuTween.WaitForCompletion();
            Activate();
        }
    }
    
    [Button]
    public void Close() {
        TargetPosition = ClosePosition;
#if UNITY_EDITOR
        if (!Application.isPlaying) {
            Rect.anchoredPosition = TargetPosition;
            return;
        }
#endif
        Deactivate();
    }

    private void Activate() {
        CanvasGroup.blocksRaycasts = true;
        State = MenuState.Open;
    }

    private void Deactivate() {
        CanvasGroup.blocksRaycasts = false;
        State = MenuState.Closed;
        StartCoroutine(DoClose());
        IEnumerator DoClose() {
            Tween menuTween = Rect.DOAnchorPos(TargetPosition, Speed)
                .SetEase(EaseType);
            yield return menuTween.WaitForCompletion();
        }
    }
}