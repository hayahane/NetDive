using DG.Tweening;
using NetDive.NetForm;
using UnityEngine;
using UnityEngine.UI;

namespace NetDive.UI
{
    public class AimTarget : MonoBehaviour
    {
        [SerializeField] private Image lockImage;
        [SerializeField] private RectTransform lockTransform;
        [SerializeField] private float initScale = 1.5f;
        [SerializeField] private Image iconImage;

        private bool _isLocked;

        public RectTransform RectTrans { get; private set; }

        private void Awake()
        {
            RectTrans = GetComponent<RectTransform>();
        }
        
        public void Lock()
        {
            if (_isLocked) return;
            
            _isLocked = true;
            lockImage.enabled = true;
            lockTransform.localScale = Vector3.one * initScale;
            lockTransform.DOScale(1, 0.5f).SetEase(Ease.InCubic).SetUpdate(UpdateType.Normal, true);
        }

        public void Unlock()
        {
            _isLocked = false;
            lockImage.enabled = false;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void SetIcon(NetFormType type)
        {
            iconImage.sprite = NetFormSystem.Instance.GetIcon(type);
            iconImage.enabled = type != NetFormType.None;
        }

        public void StartDisconnect()
        {
            iconImage.DOFillAmount(0, 1f).SetUpdate(UpdateType.Normal, true);
        }
        
        public void StopDisconnect()
        {
            DOTween.Kill(iconImage);
            iconImage.fillAmount = 1;
        }
    }
}