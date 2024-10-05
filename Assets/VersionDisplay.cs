using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class VersionDisplay : MonoBehaviour {
    [SerializeField] [BoxGroup("Settings")]
    private float VersionScale = 34f;
    
    [SerializeField][BoxGroup("Dependencies")] [ReadOnly]
    private TextMeshProUGUI VersionText;

    #if UNITY_EDITOR
    private void OnValidate() {
        if(TryGetComponent(out TextMeshProUGUI t))
        {
            VersionText = t;
        }
    }
#endif

    private void Awake() {
        SetText();
    }

    private void SetText() {
        VersionText.text = $"Knight's Chess\n<size={VersionScale}>{Application.version}</size>";
    }
}
