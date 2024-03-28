using UnityEngine;
using UnityEngine.Serialization;

namespace NetDive.NetForm
{
    public class NetFormSubstance : NetFormComponent
    {
        [SerializeField] private bool initSubstance = true;
        private bool _isSubstance;
        [field: SerializeField] public Collider Collider { get; set; }
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material virtualMaterial;
        private Material _initMaterial;

        private void OnEnable()
        {
            _isSubstance = initSubstance;
            if (meshRenderer == null) return;
            _initMaterial = meshRenderer.material;
            if (!_isSubstance)
                meshRenderer.material = virtualMaterial;
        }

        public override bool CanHandle(NetFormType type)
        {
            return type is NetFormType.Substance or NetFormType.Virtual;
        }

        public override void Connect(NetFormType type)
        {
            Collider.enabled = type == NetFormType.Substance;
            meshRenderer.material = Collider.enabled == false ? virtualMaterial : _initMaterial;
        }

        public override void Disconnect(NetFormType type)
        {
            Collider.enabled = initSubstance;
            meshRenderer.material = Collider.enabled == false ? virtualMaterial : _initMaterial;
        }
    }
}