using System.Collections.Generic;
using NetDive.Utilities.Singleton;
using UnityEngine;

namespace NetDive.NetForm
{
    public class NetFormSystem : SingletonPersistent<NetFormSystem>
    {
        public Dictionary<Collider, NetFormInstance> Instances { get; private set; } = new();
    }
}