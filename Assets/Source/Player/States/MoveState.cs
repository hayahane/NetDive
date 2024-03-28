using Cinemachine.Utility;
using UnityEngine;

namespace NetDive.Player.States
{
    public class MoveState : PlayerState
    {
        public MoveState(PlayerStateMachine stateMachine, PlayerCharacterController characterController) :
            base(stateMachine, characterController)
        {
        }

        public override void OnEnter()
        {
            characterController.IsRunning = false;
            characterController.BeginJump = false;
            characterController.AnimationController.Move();
        }

        public override void Update()
        {
            if (!characterController.Motor.GroundingStatus.IsStableOnGround)
            {
                stateMachine.TransitTo("FallState");
                return;
            }

            if (characterController.BeginJump)
            {
                stateMachine.TransitTo("JumpState");
                return;
            }

            if (characterController.MoveDirection.sqrMagnitude < 0.01f)
            {
                stateMachine.TransitTo("IdleState");
                return;
            }
            if (characterController.IsAttacking)
            {
                stateMachine.TransitTo("AttackState");
            }
        }

        public override void FixedUpdate()
        {
            var velocity = characterController.Motor.BaseVelocity;
            characterController.AnimationController.SetMoveSpeed(Mathf.Clamp(velocity.magnitude, movementData.MoveSpeed, movementData.RunSpeed));

            // Rotation
            var horizontalDirection = velocity.ProjectOntoPlane(characterController.Motor.CharacterUp).normalized;
            if (horizontalDirection == Vector3.zero) return ;
            var targetRot =
                Quaternion.LookRotation(horizontalDirection.normalized,
                    characterController.Motor.CharacterUp);
            characterController.Armature.rotation = Quaternion.Slerp(characterController.Armature.rotation, targetRot,
                movementData.Acceleration * Time.fixedDeltaTime);
        }

        public override void OnExit()
        {
        }

        public override void UpdateVelocity(ref Vector3 velocity,float deltaTime)
        {
            //Debug.Log(velocity);
            var desiredVelocity = characterController.MoveDirection *
                                  (characterController.IsRunning ? movementData.RunSpeed : movementData.MoveSpeed);
            var deltaVelocity = desiredVelocity - velocity;
            velocity += deltaVelocity.normalized *
                        Mathf.Min(
                            movementData.Acceleration *
                            (1 + movementData.Curve.Evaluate(
                                0.5f + 0.5f * Vector3.Dot(velocity, desiredVelocity))) * deltaTime,
                            deltaVelocity.magnitude);
        }

        public override void UpdateRotation(ref Quaternion rotation, float deltaTime)
        {
        }
    }
}