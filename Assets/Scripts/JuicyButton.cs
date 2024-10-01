using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class JuicyButton : SerializedMonoBehaviour, IPointerEnterHandler, IPointerDownHandler, IPointerUpHandler,
    IPointerExitHandler {
    [SerializeField] [FoldoutGroup("Settings")]
    private bool AllowReleaseEventsOnClick;

    [SerializeField] [FoldoutGroup("Settings")]
    private bool PlayReleaseEffectsOnClick;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private bool RequireHoldTime;
    
    [SerializeField] [FoldoutGroup("Settings")] [ShowIf("RequireHoldTime")]
    private float HoldTime;
    
    [SerializeField] [FoldoutGroup("Settings")] [ShowIf("RequireHoldTime")]
    private bool ForceReleaseWhenHeldTimeIsMet;

    [SerializeField] [FoldoutGroup("Settings")]
    private List<JuicyButtonEffect> PointerUpEffects;

    [SerializeField] [FoldoutGroup("Settings")]
    private List<JuicyButtonEffect> PointerDownEffects;

    [SerializeField] [FoldoutGroup("Settings")]
    private List<JuicyButtonEffect> PointerClickEffects;

    [SerializeField] [FoldoutGroup("Settings")]
    private List<JuicyButtonEffect> PointerExitEffects;

    [SerializeField] [FoldoutGroup("Events")]
    private UnityEvent OnButtonUp;

    [SerializeField] [FoldoutGroup("Events")]
    private UnityEvent OnButtonDown;

    [SerializeField] [FoldoutGroup("Events")]
    private UnityEvent OnButtonClick;

    [SerializeField] [FoldoutGroup("Events")]
    private UnityEvent OnButtonExit;

    [SerializeField] [FoldoutGroup("Status")] [ReadOnly]
    private bool IsMouseOver;
    
    [SerializeField] [FoldoutGroup("Settings")] [ReadOnly] [ShowIf("RequireHoldTime")]
    private float StartHoldTime;
    
    [SerializeField] [FoldoutGroup("Settings")] [ReadOnly] [ShowIf("RequireHoldTime")]
    private bool IsHolding;

    private void OnEnable() {
        if (!RequireHoldTime) {
            ForceReleaseWhenHeldTimeIsMet = false;
        }
    }

    private Coroutine ForceReleaseCoroutine;

    public void OnPointerEnter(PointerEventData eventData) {
        print("Mouse Enter");
        IsMouseOver = true;
    }

    public void OnPointerDown(PointerEventData eventData) {
        print("Mouse Down");
        OnButtonDown.Invoke();
        IsHolding = true;
        if (RequireHoldTime) {
            StartHoldTime = Time.time;
        }

        if (ForceReleaseWhenHeldTimeIsMet) {
            ForceReleaseCoroutine = StartCoroutine(ForceReleaseDelay());
            IEnumerator ForceReleaseDelay() {
                yield return new WaitForSeconds(HoldTime);
                Release();
            }
        }
        foreach (var downEffect in PointerDownEffects) {
            downEffect.Play();
        }
    }

    public void OnPointerUp(PointerEventData eventData) {
        if(IsHolding)
            Release();
    }

    private void Release() {
        IsHolding = false;

        if (ForceReleaseCoroutine != null) {
            StopCoroutine(ForceReleaseCoroutine);
            ForceReleaseCoroutine = null;
        }
        
        if (!IsMouseOver || (IsMouseOver && AllowReleaseEventsOnClick))
            OnButtonUp.Invoke();
        if (!IsMouseOver || (IsMouseOver && PlayReleaseEffectsOnClick))
            foreach (var upEffect in PointerUpEffects) {
                upEffect.Play();
            }

        if (!IsMouseOver) return;
        if (RequireHoldTime && Time.time < StartHoldTime + HoldTime) return;
        
        print("Click!");
        OnButtonClick.Invoke();
        foreach (var clickEffect in PointerClickEffects) {
            clickEffect.Play();
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        IsMouseOver = false;
        OnButtonExit.Invoke();
        foreach (var exitEffect in PointerExitEffects) {
            exitEffect.Play();
        }
    }
}

[Serializable]
public abstract class JuicyButtonEffect {
    [SerializeField] [FoldoutGroup("Settings")]
    protected Ease EaseType = Ease.InOutSine;

    [SerializeField] [FoldoutGroup("Settings")]
    protected float Speed = 1;

    [SerializeField] [FoldoutGroup("Settings")]
    protected float Overshoot;

    [SerializeField] [FoldoutGroup("Dependencies")]
    protected RectTransform Rect;

    public virtual void Play() {
    }
}

public class ChangeSpriteButtonEffect : JuicyButtonEffect {
    [SerializeField] private Image Img;
    [SerializeField] private Sprite Sprite;
    public override void Play() {
        Img.sprite = Sprite;
    }
}

public class PulseJuicyButtonEffect : JuicyButtonEffect {
    [SerializeField] [FoldoutGroup("Settings")]
    private Vector3 PulseEndSize;

    public override void Play() {
        Rect.DOScale(PulseEndSize, Speed)
            .SetEase(EaseType, Overshoot);
    }
}

public class RotateJuicyButtonEffect : JuicyButtonEffect {
    [SerializeField] [FoldoutGroup("Settings")]
    private RotateMode RotateMode;
    
    [SerializeField] [FoldoutGroup("Settings")]
    private Vector3  TargetAngle; 
    
    public override void Play() {
        Rect.DORotate(TargetAngle, Speed, RotateMode)
            .SetEase(EaseType, Overshoot);
    }
}

public class MoveJuicyButtonEffect : JuicyButtonEffect {
    [SerializeField] [FoldoutGroup("Settings")]
    private Vector3  TargetPosition;
    
    public override void Play() {
        Rect.DOAnchorPos3D(TargetPosition, Speed)
            .SetEase(EaseType, Overshoot);
    }
}