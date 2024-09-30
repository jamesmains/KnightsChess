using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;

public class GameTile : MonoBehaviour {
    [SerializeField] [BoxGroup("Dependencies")]
    private Image TileImage;
    
    [SerializeField] [BoxGroup("Dependencies")]
    public RectTransform TileRect;

    [SerializeField] [BoxGroup("Dependencies")]
    private GameObject MoveToButton;
    
    [SerializeField] [BoxGroup("Status")] [ReadOnly]
    public Vector2 TilePosition;
    
    [SerializeField] [BoxGroup("Status")] [ReadOnly]
    private Entity OccupyingEntity;

    [SerializeField] [BoxGroup("Status")] [ReadOnly]
    private Entity MovingEntity;

    public void SetImageColor(Color color) {
        TileImage.color = color;
    }

    public void SetTilePosition(Vector2 position) {
        TilePosition = position;
    }

    public void SetMoveValidity(Entity movingEntity) {
        MovingEntity = movingEntity;
        MoveToButton.SetActive(MovingEntity != null);
    }

    public void SetOccupyingEntity(Entity occupyingEntity) {
        if (OccupyingEntity != occupyingEntity && OccupyingEntity != null && occupyingEntity != null) {
            OccupyingEntity.JumpOffBoard();
            GameManager.CapturePiece(OccupyingEntity);
        }
        OccupyingEntity = occupyingEntity;
    }

    public void MoveToTile() {
        if (MovingEntity == null) return;
        MovingEntity.MoveToTile(this);
    }
}
