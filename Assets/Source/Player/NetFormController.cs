using System;
using System.Collections.Generic;
using DG.Tweening;
using NetDive.NetForm;
using UnityEngine;

namespace NetDive.Player
{
    public class NetFormController : MonoBehaviour
    {
        [SerializeField] private Material scanMaterial;
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
        private static readonly int ScanEnabled = Shader.PropertyToID("_ScanEnabled");
        private static readonly int ScanOrigin = Shader.PropertyToID("_ScanOrigin");
        private static readonly int ScanRange = Shader.PropertyToID("_ScanRange");
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
            if (SourceInHand == null) return;
            scanMaterial.SetFloat(ScanEnabled, 1f);
            scanMaterial.SetVector(ScanOrigin, SourceInHand.transform.position);
            scanMaterial.DOFloat(SourceInHand.Range, ScanRange, 0.5f).SetUpdate(UpdateType.Normal, true);
        }

        public void DeselectSource()
        {
            SourceInHand = null;
            scanMaterial.SetFloat(ScanEnabled, 0);
            scanMaterial.SetFloat(ScanRange, 0);
            DOTween.Kill(scanMaterial);
        }

        public void LockNetForm(int lockIndex)
        {
            _lockedInstance = ScannedNetForms[lockIndex];
        }

        public void ConnectInstance()
        {
            if (SourceInHand == null || _lockedInstance == null) return;

            if (_lockedInstance.Source != null) _lockedInstance.Source.RemoveInstance(_lockedInstance);
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