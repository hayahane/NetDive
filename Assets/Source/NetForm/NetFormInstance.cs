using System.Linq;
using UnityEngine;

namespace NetDive.NetForm
{
    [RequireComponent(typeof(SphereCollider))]
    public class NetFormInstance : MonoBehaviour
    {
        private NetFormComponent[] _components;

        private NetFormType NetFormType { get; set; } = NetFormType.None;
        public NetFormSource Source { get; set; }
        private NetFormConnection Connection { get; set; }

        private SphereCollider _collider;

        #region MonoBehaviour Callbacks

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

        #endregion
        

        public void EnableNetForm()
        {
            if (Source is null) return;
            
            NetFormType = Source.SourceType;
            Connection.Show();
            foreach (var component in _components.Where(component => component.CanHandle(Source.SourceType)))
            {
                component.Connect(Source.SourceType);
            }
        }

        public void DisableNetForm()
        {
            Connection.Hide();
            foreach (var component in _components.Where(component => component.CanHandle(NetFormType)))
            {
                component.Disconnect(NetFormType.None);
            }

            NetFormType = NetFormType.None;
        }

        public void RemoveConnection()
        {
            NetFormSystem.Instance.ReleaseConnection(Connection);
            Connection = null;
            Source = null;
        }

        public void AddConnection(NetFormSource source)
        {
            if (source is null) return;
            
            Connection = NetFormSystem.Instance.GetConnection(this, source);
            Connection.Range = source.Range;
            Source = source;
        }
    }
}