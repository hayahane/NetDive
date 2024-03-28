using System;
using System.Collections.Generic;
using Animancer;
using UnityEngine;

namespace NetDive.Player
{
    [Serializable]
    public struct AttackAnimationData
    {
        public ClipTransition clip;
        public float damage;
        public float timeForInput;
    }
    
    [CreateAssetMenu]
    public class AttackData : ScriptableObject
    {
        [field: SerializeField] public List<AttackAnimationData> AttackAnimations { get; private set; }
    }
}