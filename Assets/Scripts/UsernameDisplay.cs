using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class UsernameDisplay : MonoBehaviour
{
    [SerializeField] [FoldoutGroup("Dependencies")]
    private TMP_InputField UsernameInput;

    private void Start() {
        Reset();
    }

    [Button]
    public void Reset() {
        UsernameInput.text = UsernameManager.Singleton.CurrentUsername;
    }
    
    [Button]
    public void RandomizeUsername() {
        UsernameInput.text = UsernameManager.GetRandomUsername();
    }
    
    [Button]
    public void ConfirmUsername() {
        UsernameManager.SetUsername(UsernameInput.text);
    }
}
