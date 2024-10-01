using Sirenix.OdinInspector;
using UnityEngine;

public class AudioTrigger : MonoBehaviour
{
    [Button]
    public void PlaySfx() {
        AudioManager.PlayUISfxClip();
    }
}
