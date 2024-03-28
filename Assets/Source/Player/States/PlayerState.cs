using NetDive.Utilities.StateMachine;
using UnityEngine;

namespace NetDive.Player.States
{
    public abstract class PlayerState : IState
    {
        protected readonly PlayerStateMachine stateMachine;
        protected readonly PlayerCharacterController characterController;
        protected readonly PlayerMovementData movementData;

        protected PlayerState(PlayerStateMachine stateMachine, PlayerCharacterController characterController)
        {
            this.stateMachine = stateMachine;
            this.characterController = characterController;
            this.movementData = characterController.MovementData;
        }

        public abstract void UpdateVelocity(ref Vector3 velocity, float deltaTime);
        public abstract void UpdateRotation(ref Quaternion rotation, float deltaTime);

        public abstract void OnEnter();
        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void OnExit();
    }
}