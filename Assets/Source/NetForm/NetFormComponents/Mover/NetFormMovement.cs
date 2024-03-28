using KinematicCharacterController;
using Unity.VisualScripting;
using UnityEngine;
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

    public enum HitWallMode
    {
        Bounce,
        Return
    }

    public class NetFormMovement : NetFormComponent, IMoverController
    {
        [SerializeField] private bool initMoving = true;
        private bool _isMoving;


        private bool _hitObstacle;
        private float _backTime;
        private float _playDirection = 1;
        [field: SerializeField] public HitWallMode HitWallMode { get; set; } = HitWallMode.Bounce;
        [field: SerializeField] public float GoBackTime { get; set; } = 0.1f;
        [field: SerializeField] public float BackTimeScale { get; set; } = 0.1f;
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

        #region NetFormComponent Implementation

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

        #endregion

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

            var dt = deltaTime * _playDirection;
            if (_hitObstacle)
            {
                switch (HitWallMode)
                {
                    case HitWallMode.Bounce:
                        _backTime -= deltaTime;
                        if (_backTime <= 0)
                        {
                            _hitObstacle = false;
                            _playDirection *= -1;
                        }
                        dt *= BackTimeScale;
                        break;
                }
            }

            var t = CalculateNormalizedTime(dt);
            var pos = spline.EvaluatePosition(_splinePath, t);
            var motion = (Vector3)pos - mover.transform.position;
            if (mover.Rigidbody.SweepTest(motion.normalized, out var hit, motion.magnitude) &&
                !hit.collider.gameObject.CompareTag("Player"))
            {
                _hitObstacle = true;
                _backTime = GoBackTime;
                _playDirection *= -1;
            }

            goalPosition = pos;
            goalRotation = Quaternion.identity;
        }
    }
}