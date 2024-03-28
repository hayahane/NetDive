using Animancer;
using UnityEngine;
using NetDive.NetForm;

namespace NetDive.Player.Animation
{
    [CreateAssetMenu(menuName = "Player/AnimationData", fileName = "New AnimationData")]
    public class AnimationData : ScriptableObject
    {
        [field: SerializeField] public ClipTransition Idle { get; set; }
        [field: SerializeField] public LinearMixerTransition Walk { get; set; }
        [field: SerializeField] public ClipTransition JumpStart { get; set; }
        [field: SerializeField] public ClipTransition JumpLoop { get; set; }
    }
}