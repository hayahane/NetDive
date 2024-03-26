using System.Linq;
using UnityEngine;

namespace NetDive.NetForm
{
    [RequireComponent(typeof(SphereCollider))]
    public class NetFormInstance : MonoBehaviour
    {
        private NetFormComponent[] _components;

        public NetFormSource Source { get; set; }
        private SphereCollider _collider;
        
        private void OnEnable()
        {
            _collider = GetComponent<SphereCollider>();
            _collider.isTrigger = true;
            _components = GetComponents<NetFormComponent>();
            gameObject.layer = LayerMask.NameToLayer("NetForm");
            NetFormSystem.Instance.Instances.Add(_collider, this);
        }

        private void OnDisable()
        {
            if (gameObject.scene.isLoaded == false) return;
            NetFormSystem.Instance.Instances.Remove(_collider);
        }

        public void NetFormConnect(NetFormType type)
        {
            foreach (var component in _components.Where(component => component.CanHandle(type)))
            {
                component.Connect(type);
            }
        }

        public void NetFormDisconnect(NetFormType type)
        {
            foreach (var component in _components.Where(component => component.CanHandle(type)))
            {
                component.Disconnect(type);
            }
        }
    }
}