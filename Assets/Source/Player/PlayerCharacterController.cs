using KinematicCharacterController;
using NetDive.Utilities.StateMachine;
using NetDive.Player.Animation;
using NetDive.Player.States;
using Unity.VisualScripting;
using UnityEngine;

namespace NetDive.Player
{
    public class PlayerStateMachine : StateMachine<PlayerState>
    {
        public PlayerStateMachine(PlayerCharacterController characterController)
        {
            AddState(new MoveState(this, characterController), "MoveState");
            AddState(new JumpState(this, characterController), "JumpState");
            AddState(new FallState(this, characterController), "FallState");
            AddState(new IdleState(this, characterController), "IdleState");
            AddState(new AttackState(this, characterController), "AttackState");

            TransitTo("IdleState");
        }
    }

    public class PlayerCharacterController : MonoBehaviour, ICharacterController
    {
        [field: SerializeField] public PlayerMovementData MovementData { get; private set; }
        [field: SerializeField] public AttackData AttackData { get; private set; }
        [field: SerializeField] public Transform Armature { get; private set; }
        [field: SerializeField] public AnimationController AnimationController { get; private set; }

        public bool IsRunning { get; set; }
        public Vector3 MoveDirection { get; set; }
        public bool BeginJump { get; set; }
        public bool IsJumping { get; set; }
        public bool IsAttacking { get; set; }

        public KinematicCharacterMotor Motor { get; private set; }
        private PlayerStateMachine _psm;
        private ICharacterController _characterControllerImplementation;

        private void Start()
        {
            gameObject.tag = "Player";
            _psm = new PlayerStateMachine(this);
        }

        private void OnEnable()
        {
            Motor = GetComponent<KinematicCharacterMotor>();
            Motor.CharacterController = this;
        }

        private void Update()
        {
            _psm.Update();
        }

        private void FixedUpdate()
        {
            _psm.FixedUpdate();
        }

        #region CharacterController Implementation

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            _psm.CurrentState.UpdateRotation(ref currentRotation, deltaTime);
        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            _psm.CurrentState.UpdateVelocity(ref currentVelocity, deltaTime);
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
        }

        public void PostGroundingUpdate(float deltaTime)
        {
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            return true;
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            ref HitStabilityReport hitStabilityReport)
        {
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint,
            Vector3 atCharacterPosition,
            Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
        }

        #endregion
    }
}