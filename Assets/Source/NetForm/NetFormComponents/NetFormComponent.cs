using UnityEngine;

namespace NetDive.NetForm
{
    public abstract class NetFormComponent : MonoBehaviour
    {
        public abstract bool CanHandle(NetFormType type);
        public abstract void Connect(NetFormType type);
        public abstract void Disconnect(NetFormType type);
    }
}