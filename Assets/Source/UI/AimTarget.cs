using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace NetDive.UI
{
    public class AimTarget : MonoBehaviour
    {
        [SerializeField] private Image lockImage;
        [SerializeField] private RectTransform lockTransform;
        [SerializeField] private float initScale = 1.5f;

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
            lockTransform.DOScale(1, 0.5f).SetEase(Ease.InCubic);
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
    }
}