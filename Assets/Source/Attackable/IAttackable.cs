using NetDive.Player;
using UnityEngine;

namespace NetDive.Attackable
{
    public interface IAttackable
    {
        public void OnAttack(Transform attacker, float damage);
    }
}