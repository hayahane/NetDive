using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Splines;

namespace NetDive.NetForm
{
    public enum MoveMode
    {
        Spline,
        Playable
    }

    public enum LoopMode
    {
        Once,
        PingPong,
        Loop
    }

    public enum EaseMode
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }

    public class NetFormMovement : NetFormComponent, IMoverController
    {
        [SerializeField] private bool initMoving = true;
        private bool _isMoving;
        
        [field: SerializeField] public bool GoBackWhenHitWall { get; set; }
        [field: SerializeField] public LoopMode LoopMode { get; set; } = LoopMode.Once;
        [field: SerializeField] public EaseMode EaseMode { get; set; } = EaseMode.Linear;
        [field: SerializeField] public float Duration { get; set; }
        private float _elapsedTime;

        [SerializeField] private SplineContainer spline;

        public SplineContainer Spline
        {
            get => spline;
            set
            {
                spline = value;
                if (spline != null) RebuildSplinePath();
            }
        }

        private SplinePath<Spline> _splinePath;
        [SerializeField] private PhysicsMover mover;

        public void OnEnable()
        {
            _isMoving = initMoving;
            mover.MoverController = this;
            RebuildSplinePath();
        }

        public override bool CanHandle(NetFormType type)
        {
            return type is NetFormType.Motion or NetFormType.Freeze;
        }

        public override void Connect(NetFormType type)
        {
            _isMoving = type == NetFormType.Motion;
        }

        public override void Disconnect(NetFormType type)
        {
            _isMoving = initMoving;
        }

        private void RebuildSplinePath()
        {
            _splinePath = new SplinePath<Spline>(Spline.Splines);
        }

        private float CalculateNormalizedTime(float deltaTime)
        {
            _elapsedTime += deltaTime;
            var duration = Duration;

            var t = .0f;
            switch (LoopMode)
            {
                case LoopMode.Once:
                    t = Mathf.Min(_elapsedTime, duration);
                    break;
                case LoopMode.Loop:
                    t = _elapsedTime % duration;
                    break;
                case LoopMode.PingPong:
                    t = Mathf.PingPong(_elapsedTime, duration);
                    break;
                default:
                    Debug.LogError("Unsupported LoopMode");
                    break;
            }

            t /= duration;

            switch (EaseMode)
            {
                case EaseMode.EaseIn:
                    t = t * t;
                    break;
                case EaseMode.EaseOut:
                    t = t * (2f - t);
                    break;
                case EaseMode.EaseInOut:
                    var eased = 2f * t * t;
                    if (t > 0.5f)
                        eased = 4f * t - eased - 1f;
                    t = eased;
                    break;
            }

            return t;
        }


        public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
        {
            if (!_isMoving)
            {
                goalPosition = transform.position;
                goalRotation = transform.rotation;
                return;
            }

            var t = CalculateNormalizedTime(deltaTime);
            var pos = spline.EvaluatePosition(_splinePath, t);

            goalPosition = pos;
            goalRotation = Quaternion.identity;
        }
    }
}