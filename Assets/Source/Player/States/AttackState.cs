using UnityEngine;

namespace NetDive.Player.States
{
    public class AttackState : PlayerState
    {
        private AttackData _data;
        private float _timeSinceEnter;
        private int _currentStage = -1;
        private bool _isNextAvailable;
        private float _currentInputTime;
        private float _currentDuration;

        public AttackState(PlayerStateMachine stateMachine, PlayerCharacterController characterController) : base(
            stateMachine, characterController)
        {
        }

        public override void UpdateVelocity(ref Vector3 velocity, float deltaTime)
        {
        }

        public override void UpdateRotation(ref Quaternion rotation, float deltaTime)
        {
        }

        public override void OnEnter()
        {
            characterController.Motor.BaseVelocity = Vector3.zero;
            characterController.AnimationController.ApplyRootMotion = true;
            _data = characterController.AttackData;
            _currentStage = -1;

            GoToNetAttackStage();
        }

        private void GoToNetAttackStage()
        {
            _isNextAvailable = false;
            _currentStage++;
            characterController.AnimationController.Attack(_data.AttackAnimations[_currentStage].clip);
            _timeSinceEnter = 0;
            characterController.IsAttacking = false;
            _currentInputTime = _data.AttackAnimations[_currentStage].timeForInput;
            _currentDuration = characterController.AnimationController.GetCurrentDuration();
        }

        public override void Update()
        {
            if (!characterController.Motor.GroundingStatus.IsStableOnGround)
            {
                stateMachine.TransitTo("FallState");
                return;
            }
            _timeSinceEnter += Time.deltaTime;
            if (characterController.IsAttacking && _timeSinceEnter < _data.AttackAnimations[_currentStage].timeForInput
                                                && _currentStage < _data.AttackAnimations.Count - 1)
            {
                _isNextAvailable = true;
            }

            if (_timeSinceEnter < _currentInputTime) return;

            if (_isNextAvailable)
            {
                GoToNetAttackStage();
            }
            else
            {
                if (_timeSinceEnter >= _currentDuration * 0.9f)
                    stateMachine.TransitTo("IdleState");
            }
        }

        public override void FixedUpdate()
        {
        }

        public override void OnExit()
        {
            characterController.IsAttacking = false;
            characterController.AnimationController.ApplyRootMotion = false;
        }
    }
}