using System.Collections.Generic;
using NetDive.NetForm;
using UnityEngine;

namespace NetDive.Player
{
    public class NetFormController : MonoBehaviour
    {
        [field: SerializeField] public float ScanRange { get; set; } = 10f;

        public int ScannedCount { get; private set; }
        public Collider[] ScannedColliders { get; } = new Collider[32];
        
        public NetFormInstance LockedNetForm { get; private set; }
        [field: SerializeField] public List<NetFormInstance> ScannedNetForms { get; private set; } = new();

        public void ScanNetForm()
        {
            ScannedNetForms.Clear();
            
            ScannedCount =
                Physics.OverlapSphereNonAlloc(transform.position, ScanRange,
                    ScannedColliders, 1 << LayerMask.NameToLayer("NetForm"));

            for (var i = 0; i < ScannedCount; i++)
            {
                if (NetFormSystem.Instance.Instances.TryGetValue(ScannedColliders[i],out var instance))
                {
                    ScannedNetForms.Add(instance);
                }
            }
        }

        public void LocketForm(int lockIndex)
        {
            LockedNetForm = ScannedNetForms[lockIndex];
        }
        
        private void FixedUpdate()
        {
            ScanNetForm();
        }
    }
}