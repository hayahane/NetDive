using System;
using UnityEngine;

namespace NetDive.NetForm
{
    public class NetFormRigidBody : NetFormComponent
    {
        [SerializeField] private Rigidbody rb;
        private RigidbodyConstraints _constraints;

        private void OnEnable()
        {
            _constraints = rb.constraints;
        }

        public override bool CanHandle(NetFormType type)
        {
            return type is NetFormType.Static;
        }

        public override void Connect(NetFormType type)
        {
            
            rb.constraints = RigidbodyConstraints.FreezeAll;
        }

        public override void Disconnect(NetFormType type)
        {
            rb.constraints = _constraints;
        }
    }
}