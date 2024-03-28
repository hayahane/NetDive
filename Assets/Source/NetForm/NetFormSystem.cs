using System;
using System.Collections.Generic;
using NetDive.Utilities.Singleton;
using UnityEngine;
using UnityEngine.Pool;

namespace NetDive.NetForm
{
    public class NetFormSystem : SingletonPersistent<NetFormSystem>
    {
        [field: SerializeField] public NetFormSettings Settings { get; private set; }
        public Dictionary<Collider, NetFormInstance> Instances { get; private set; } = new();
        public Dictionary<Collider, NetFormSource> Sources { get; private set; } = new();
        
        [SerializeField] private GameObject dataExpression;
        

        private readonly ObjectPool<NetFormConnection> _connectionPool = new(
            () => Instance.CreateConnection(),
            connection => connection.gameObject.SetActive(true),
            connection =>
            {
                connection.transform.parent = Instance.transform;
                connection.gameObject.SetActive(false);
            }, Destroy);

        public Sprite GetIcon(NetFormType type)
        {
            return Settings.GetIcon(type);
        }

        public Color GetColor(NetFormType type)
        {
            if (type == NetFormType.None) return Color.black;
            return Settings.netFormColors[(int)type / 2];
        }

        public NetFormConnection GetConnection(NetFormInstance instance, NetFormSource source)
        {
            var connection = _connectionPool.Get();
            var sourceTrans = source.transform;
            connection.Start = sourceTrans;
            connection.End = instance.transform;
            connection.transform.parent = sourceTrans;
            return connection;
        }

        public void ReleaseConnection(NetFormConnection connection)
        {
            _connectionPool.Release(connection);
        }
        
        private NetFormConnection CreateConnection()
        {
            var connection = Instantiate(dataExpression).AddComponent<NetFormConnection>();
            return connection;
        }
    }
}