using UnityEngine;

namespace NetDive.NetForm
{
    public class NetFormConnection : MonoBehaviour
    {
        public float Range { get; set; }
        private Transform _start;

        public Transform Start
        {
            get => _start;
            set
            {
                if (value == null) return;
                _start = value;
            }
        }

        private Transform _end;

        public Transform End
        {
            get => _end;
            set
            {
                if (value == null) return;
                _end = value;
            }
        }

        private Material _material;


        private static readonly int Scale = Shader.PropertyToID("_Scale");
        private static readonly int Distance = Shader.PropertyToID("_Distance");
        private static readonly int Brightness = Shader.PropertyToID("_Brightness");

        private void OnEnable()
        {
            _material = GetComponent<MeshRenderer>().material;
        }

        private void Update()
        {
            if (_start is null || _end is null) return;

            var position = _start.position;
            var position1 = _end.position;
            var length = Vector3.Distance(position, position1);
            var tf = transform;
            tf.position = (position + position1) / 2;
            tf.localScale = new Vector3(0.1f, length / 2f, 0.1f);
            tf.up = -(position1 - position).normalized;

            _material.SetVector(Scale, new Vector2(0.1f, 0.1f * length / 0.2f));
            _material.SetFloat(Distance, length / Range);
        }
        
        public void Hide()
        {
            _material.SetFloat(Brightness, -60f);
        }
        
        public void Show()
        {
            _material.SetFloat(Brightness, 30f);
        }
    }
}