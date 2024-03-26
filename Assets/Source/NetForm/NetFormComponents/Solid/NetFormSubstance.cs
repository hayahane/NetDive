using UnityEngine;

namespace NetDive.NetForm
{
    
    public class NetFormSubstance : NetFormComponent
    {
        [SerializeField] private bool initSubstance = true;
        private bool _isSubstance;
        [field: SerializeField] public Collider Collider { get; set; }

        private void OnEnable()
        {
            _isSubstance = initSubstance;
        }

        public override bool CanHandle(NetFormType type)
        {
            return type == NetFormType.Substance || type == NetFormType.Virtual;
        }

        public override void Connect(NetFormType type)
        {
            Collider.enabled = type == NetFormType.Substance; 
        }

        public override void Disconnect(NetFormType type)
        {
            Collider.enabled = initSubstance;
        }
    }
}