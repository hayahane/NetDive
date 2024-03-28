using System;
using System.Collections.Generic;
using System.Linq;
using NetDive.Player;
using UnityEngine;

namespace NetDive.NetForm
{
    public enum SourceMode
    {
        Constant,
        Blink,
        Shutdown
    }

    public class NetFormSource : MonoBehaviour
    {
        private bool _initialized = false;

        [field: Header("Connection")]
        [field: SerializeField]
        public float Range { get; private set; } = 10f;

        [field: SerializeField] public NetFormType SourceType { get; private set; }
        [field: SerializeField] public List<NetFormInstance> Instances { get; private set; } = new();
        private readonly List<NetFormInstance> _abortedInstances = new();

        [field: Header("Source Mode")]
        [field: SerializeField]
        public SourceMode SourceMode { get; private set; } = SourceMode.Constant;

        [field: SerializeField] public float BlinkInterval { get; private set; } = 1f;
        [field: SerializeField] public float OperatingTime { get; private set; } = 5f;
        private float _elapsedTime;
        private bool _isOperating;

        private static readonly int Emission = Shader.PropertyToID("_EmissionColor");
        private static readonly int EmissionMap = Shader.PropertyToID("_EmissionMap");

        #region MonoBehaviour Callbacks

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, Range);
        }

        private void OnEnable()
        {
            var area = GetComponent<Collider>();
            area.isTrigger = true;
            var spriteRenderer = GetComponent<SpriteRenderer>();
            spriteRenderer.sprite = NetFormSystem.Instance.GetIcon(SourceType);
            spriteRenderer.material.SetColor(Emission, NetFormSystem.Instance.GetColor(SourceType));
            spriteRenderer.material.SetTexture(EmissionMap, spriteRenderer.sprite.texture);
        }

        private void OnDisable()
        {
            if (gameObject.scene.isLoaded == false) return;
            DisconnectHoldingInstance();
            _initialized = false;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            var netFormController = other.GetComponent<NetFormController>();
            if (netFormController == null) return;

            netFormController.TmpSource = this;
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.CompareTag("Player")) return;

            var netFormController = other.GetComponent<NetFormController>();
            if (netFormController == null) return;

            netFormController.DeselectSource();
            netFormController.TmpSource = null;
        }

        private void Update()
        {
            if (!_initialized)
            {
                ConnectHoldingInstance();
                _initialized = true;
            }

            foreach (var instance in Instances.Where(instance =>
                         Vector3.Distance(instance.transform.position, transform.position) > Range))
            {
                instance.DisableNetForm();
                instance.RemoveConnection();
                instance.Source = null;
                _abortedInstances.Add(instance);
            }

            if (_abortedInstances.Count > 0)
            {
                foreach (var instance in _abortedInstances)
                {
                    Instances.Remove(instance);
                }

                _abortedInstances.Clear();
            }

            switch (SourceMode)
            {
                case SourceMode.Blink:
                    _elapsedTime += Time.deltaTime;
                    _elapsedTime %= (BlinkInterval + OperatingTime);
                    if (_elapsedTime < OperatingTime && !_isOperating)
                    {
                        _isOperating = true;
                        foreach (var instance in Instances)
                        {
                            instance.EnableNetForm();
                        }

                        break;
                    }

                    if (_elapsedTime >= OperatingTime && _isOperating)
                    {
                        _isOperating = false;
                        foreach (var instance in Instances)
                        {
                            instance.DisableNetForm();
                        }
                    }

                    break;
            }
        }

        #endregion


        private void ConnectHoldingInstance()
        {
            foreach (var instance in Instances)
            {
                ConnectInstance(instance);
            }
        }

        private void DisconnectHoldingInstance()
        {
            foreach (var instance in Instances)
            {
                DisconnectInstance(instance);
            }
        }

        public void RemoveInstance(NetFormInstance instance)
        {
            DisconnectInstance(instance);
            Instances.Remove(instance);
        }

        public void AddInstance(NetFormInstance instance)
        {
            ConnectInstance(instance);
            Instances.Add(instance);
        }

        private void ConnectInstance(NetFormInstance instance)
        {
            instance.AddConnection(this);
            instance.EnableNetForm();
        }

        private void DisconnectInstance(NetFormInstance instance)
        {
            instance.DisableNetForm();
            instance.RemoveConnection();
        }
    }
}