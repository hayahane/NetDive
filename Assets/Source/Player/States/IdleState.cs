using UnityEngine;

namespace NetDive.Player.States
{
    public class IdleState : PlayerState
    {
        public IdleState(PlayerStateMachine stateMachine, PlayerCharacterController characterController) :
            base(stateMachine, characterController)
        {
        }

        public override void OnEnter()
        {
            characterController.BeginJump = false;
            characterController.AnimationController.Idle();
            characterController.Motor.BaseVelocity = Vector3.zero;
        }

        public override void Update()
        {
            if (!characterController.Motor.GroundingStatus.IsStableOnGround)
            {
                stateMachine.TransitTo("FallState");
                return;
            }

            if (characterController.MoveDirection.sqrMagnitude >= 0.001f)
            {
                stateMachine.TransitTo("MoveState");
                return;
            }

            if (characterController.BeginJump)
            {
                stateMachine.TransitTo("JumpState");
                return;
            }

            if (characterController.IsAttacking)
            {
                stateMachine.TransitTo("AttackState");
            }
        }

        public override void FixedUpdate()
        {
        }

        public override void OnExit()
        {
        }

        public override void UpdateVelocity(ref Vector3 velocity, float deltaTime)
        {
            velocity = Vector3.zero;
        }

        public override void UpdateRotation(ref Quaternion quaternion, float deltaTime)
        {
        }
    }
}