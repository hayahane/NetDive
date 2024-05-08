using System;
using Cinemachine.Utility;
using UnityEngine;

namespace NetDive.Attackable
{
    public class AttackableRigidBody : MonoBehaviour, IAttackable
    {
        [SerializeField] private float forceScale = 5f;
        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        public void OnAttack(Transform attacker, float damage)
        {
            Debug.Log("Attacked");
            _rb.AddForce(attacker.forward.ProjectOntoPlane(Vector3.up).normalized * forceScale, ForceMode.VelocityChange);
        }
    }
}