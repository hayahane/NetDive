using System;
using UnityEngine;

namespace NetDive.Enemy
{
    public class EnemyTest : MonoBehaviour
    {
        private Rigidbody _rb;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            _rb.WakeUp();
        }
    }
}