using Cinemachine.Utility;
using UnityEngine;

namespace NetDive.Player.States
{
    public class FallState : PlayerState
    {
        private Vector3 _forward;
        public FallState(PlayerStateMachine stateMachine, PlayerCharacterController characterController) : 
            base(stateMachine, characterController)
        { }
        
        public override void OnEnter()
        {
            characterController.AnimationController.Fall();
            _forward = characterController.Motor.BaseVelocity.ProjectOntoPlane(characterController.Motor.CharacterUp)
                .normalized;
        }

        public override void Update()
        {
            if (characterController.Motor.GroundingStatus.IsStableOnGround)
            {
                stateMachine.TransitTo("IdleState");
            }
        }

        public override void FixedUpdate()
        {
            var velocity = characterController.Motor.BaseVelocity;

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
            var verticalVelocity = Vector3.Project(velocity, characterController.Motor.CharacterUp);
            verticalVelocity += Physics.gravity * deltaTime;
            var horizontalVelocity = characterController.MoveDirection * movementData.MoveSpeed;
            horizontalVelocity = Vector3.Project(horizontalVelocity, _forward);

            velocity = verticalVelocity + horizontalVelocity;
        }

        public override void UpdateRotation(ref Quaternion rotation, float deltaTime)
        {
        }
    }
}