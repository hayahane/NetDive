using System;
using UnityEngine;

namespace NetDive.Attackable
{
    public class AttackCollider : MonoBehaviour
    {
        public float Damage { get; set; }
        public Transform Player { get; set; }

        private void OnTriggerEnter(Collider other)
        {
            var iAttackable = other.GetComponent<IAttackable>();
            if (Player == null) return;
            iAttackable?.OnAttack(Player, Damage);
        }
    }
}