using NetDive.NetForm;
using NetDive.Player;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace NetDive.UI
{
    public class NetFormUIManager : MonoBehaviour
    {
        [Header("Aim Target")] [SerializeField]
        private GameObject aimTargetPrefab;

        [SerializeField] private GameObject aimTargetManager;
        private readonly AimTarget[] _aimTargets = new AimTarget[32];

        [FormerlySerializedAs("scanner")] [SerializeField]
        private NetFormController controller;

        [SerializeField] private Camera viewCamera;
        [SerializeField] private CanvasScaler canvas;
        [SerializeField] private CanvasGroup canvasGroup;

        private AimTarget _current;

        [Header("Source Note")]
        [SerializeField] private CanvasGroup noteCanvasGroup;

        [SerializeField] private GameObject inputNote;
        [SerializeField] private GameObject netFormNote;

        private void Start()
        {
            if (aimTargetManager == null) return;
            for (var i = 0; i < _aimTargets.Length; i++)
            {
                _aimTargets[i] = Instantiate(aimTargetPrefab, aimTargetManager.transform).GetComponent<AimTarget>();
                _aimTargets[i].Hide();
            }
        }

        private void Update()
        {
            if (controller is null) return;

            canvasGroup.alpha = controller.SourceInHand is null ? 0 : 1;
            noteCanvasGroup.alpha = controller.TmpSource is not null && controller.SourceInHand is null ? 1 : 0;
        }

        private void LateUpdate()
        {
            if (!controller || !viewCamera) return;

            if (controller.SourceInHand is null) return;

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
                var type = controller.ScannedNetForms[i].Source is null
                    ? NetFormType.None
                    : controller.ScannedNetForms[i].Source.SourceType;
                _aimTargets[i].SetIcon(type);
                _aimTargets[i].RectTrans.anchoredPosition =
                    new Vector2(pos.x * canvas.referenceResolution.x, pos.y * canvas.referenceResolution.y);

                var dist = Vector2.Distance(new Vector2(pos.x, pos.y), Vector2.zero);

                if (!(dist < minDistance)) continue;
                if (Mathf.Approximately(dist, minDistance) && nearZ <= pos.z) continue;
                nearZ = pos.z;
                minDistance = dist;
                lockIndex = i;
            }

            for (i = 0; i < controller.ScannedNetForms.Count; i++)
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

            if (lockIndex >= 0)
            {
                controller.LockNetForm(lockIndex);
                if (_aimTargets[lockIndex] != _current && _current)
                    _current.StopDisconnect();
                _current = _aimTargets[lockIndex];
            }
        }

        public void StopDisconnectCurrent()
        {
            if (_current == null) return;
            _current.StopDisconnect();
        }

        public void DisconnectCurrent()
        {
            if (_current == null) return;
            _current.StartDisconnect();
        }

        public void ChangeInputPrompt(NetFormSource source)
        {
            if (source == null)
            {
                inputNote.SetActive(true);
                netFormNote.SetActive(false);
            }
            else
            {
                inputNote.SetActive(false);
                netFormNote.SetActive(true);
            }
        }
    }
}