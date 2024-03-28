using Cinemachine.Utility;
using UnityEngine;

namespace NetDive.Player.States
{
    public class JumpState : PlayerState
    {
        private float _verticalAcceleration;
        private float _extraAcceleration;
        private float _jumpTime;
        private Vector3 _horizontalVelocity;
        private float _verticalVelocity;

        public JumpState(PlayerStateMachine stateMachine, PlayerCharacterController characterController) :
            base(stateMachine, characterController)
        {
        }

        public override void OnEnter()
        {
            characterController.BeginJump = false;
            _horizontalVelocity = characterController.MoveDirection *  movementData.MoveSpeed;
            
            _verticalAcceleration = 2 * movementData.JumpHeight / (movementData.JumpTime * movementData.JumpTime);
            _verticalVelocity = _verticalAcceleration * movementData.JumpTime;
            _jumpTime = movementData.JumpTime;
            _jumpTime *= (1 + movementData.ExtraJumpHeight / movementData.JumpHeight);
            _extraAcceleration = _verticalVelocity / _jumpTime;
            
            characterController.AnimationController.BeginJump();
        }

        public override void Update()
        {
            if (_jumpTime <= 0 || _verticalVelocity <= 0)
            {
                stateMachine.TransitTo("FallState");
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
            var a = characterController.IsJumping? _extraAcceleration : _verticalAcceleration;
            _jumpTime -= deltaTime;
            _verticalVelocity -= a * deltaTime;
            characterController.Motor.ForceUnground();
            velocity = _horizontalVelocity + characterController.Motor.CharacterUp * _verticalVelocity;
        }

        public override void UpdateRotation(ref Quaternion rotation, float deltaTime)
        {
        }
    }
}