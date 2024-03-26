using System;
using NetDive.Player;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NetDive.UI
{
    public class AimTargetManager : MonoBehaviour
    {
        [SerializeField] private GameObject aimTargetPrefab;

        private readonly AimTarget[] _aimTargets = new AimTarget[32];
        [FormerlySerializedAs("scanner")] [SerializeField] private NetFormController controller;
        [SerializeField] private Camera viewCamera;
        [SerializeField] private CanvasScaler canvas;

        private void Start()
        {
            for (var i = 0; i < _aimTargets.Length; i++)
            {
                _aimTargets[i] = Instantiate(aimTargetPrefab, transform).GetComponent<AimTarget>();
                _aimTargets[i].Hide();
            }
        }

        private void LateUpdate()
        {
            if (!controller || !viewCamera) return;

            var i = 0;
            var minDistance = float.MaxValue;
            var nearZ = float.MaxValue;
            var lockIndex = -1;
            for (; i < controller.ScannedNetForms.Count; i++)
            {
                var pos = viewCamera.WorldToViewportPoint(controller.ScannedColliders[i].transform.position);
                pos -= new Vector3(0.5f, 0.5f, 0);
                if (pos.z <= 0)
                {
                    _aimTargets[i].Hide();
                    continue;
                }
                _aimTargets[i].Show();
                _aimTargets[i].RectTrans.anchoredPosition =
                    new Vector2(pos.x * canvas.referenceResolution.x, pos.y * canvas.referenceResolution.y);

                var dist = Vector2.Distance(new Vector2(pos.x, pos.y), Vector2.zero);
                
                if (!(dist < minDistance)) continue;
                if (Mathf.Approximately(dist,minDistance) && nearZ <= pos.z) continue;
                nearZ = pos.z;
                minDistance = dist;
                lockIndex = i;
            }

            for (i =0; i < controller.ScannedNetForms.Count; i++)
            {
                if (i == lockIndex)
                {
                    _aimTargets[i].Lock();
                }
                else
                {
                    _aimTargets[i].Unlock();
                }
            }

            for (; i < _aimTargets.Length; i++)
            {
                _aimTargets[i].Unlock();
                _aimTargets[i].Hide();
            }
        }
    }
}