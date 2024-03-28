using UnityEngine;

namespace NetDive.Player
{
    [CreateAssetMenu(menuName = "Player/PlayerMovementData", fileName = "New PlayerMovementData")]
    public class PlayerMovementData : ScriptableObject
    {
        [field: SerializeField] public float MoveSpeed { get; set; } = 3f;
        [field: SerializeField] public float RunSpeed { get; set; } = 5f;
        [field: SerializeField] public float Acceleration { get; set; } = 15f;
        [field: SerializeField] public AnimationCurve Curve { get; set; } = new();

        [field: SerializeField] public float JumpHeight { get; set; } = 1;
        [field: SerializeField] public float JumpTime { get; set; } = 0.5f;
        [field: SerializeField] public float ExtraJumpHeight { get; set; } = 0.5f;
    }
}