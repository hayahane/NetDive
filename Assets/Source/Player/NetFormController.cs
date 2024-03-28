using System;
using System.Collections.Generic;
using NetDive.NetForm;
using UnityEngine;

namespace NetDive.Player
{
    public class NetFormController : MonoBehaviour
    {
        private int _scannedCount;
        public Collider[] ScannedColliders { get; } = new Collider[32];

        private NetFormSource _source;

        public NetFormSource SourceInHand
        {
            get => _source;
            private set
            {
                _source = value;
                BulletTime(_source);
                SourceChanged?.Invoke(_source);
            }
        }

        public event Action<NetFormSource> SourceChanged;
        public event Action<NetFormSource> TmpSourceChanged; 

        private NetFormSource _tmpSource;

        public NetFormSource TmpSource
        {
            get => _tmpSource;
            set
            {
                _tmpSource = value;
                TmpSourceChanged?.Invoke(_tmpSource);
            }
        }

        private NetFormInstance _lockedInstance;
        public List<NetFormInstance> ScannedNetForms { get; private set; } = new();

        private void ScanNetForm()
        {
            if (SourceInHand is null) return;

            ScannedNetForms.Clear();

            _scannedCount =
                Physics.OverlapSphereNonAlloc(SourceInHand.transform.position, SourceInHand.Range,
                    ScannedColliders, NetFormSystem.Instance.Settings.netFormLayer);

            for (var i = 0; i < _scannedCount; i++)
            {
                if (NetFormSystem.Instance.Instances.TryGetValue(ScannedColliders[i], out var instance))
                {
                    ScannedNetForms.Add(instance);
                }
            }
        }

        private void FixedUpdate()
        {
            if (SourceInHand is null) return;

            ScanNetForm();
        }

        public void SelectSource()
        {
            SourceInHand = TmpSource;
        }

        public void DeselectSource()
        {
            SourceInHand = null;
        }

        public void LockNetForm(int lockIndex)
        {
            _lockedInstance = ScannedNetForms[lockIndex];
        }

        public void ConnectInstance()
        {
            if (SourceInHand == null || _lockedInstance == null) return;
            SourceInHand.AddInstance(_lockedInstance);
        }

        public void DisconnectInstance()
        {
            if (_lockedInstance == null) return;
            if (_lockedInstance.Source == null) return;
            _lockedInstance.Source.RemoveInstance(_lockedInstance);
        }

        private static void BulletTime(NetFormSource source)
        {
            if (source != null)
            {
                Time.timeScale = 0.2f;
                //Time.fixedDeltaTime *= 0.2f;
            }
            else
            {
                Time.timeScale = 1f;
                //Time.fixedDeltaTime /= 0.2f;
            }
        }
    }
}