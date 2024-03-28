using Animancer;
using UnityEngine;


namespace NetDive.Player.Animation
{
    public class AnimationController : MonoBehaviour
    {
        [SerializeField] private AnimancerComponent animancer;
        [SerializeField] private AnimationData animationData;

        private ClipTransition _currentAttack;

        public bool ApplyRootMotion
        {
            get => animancer.Animator.applyRootMotion;
            set => animancer.Animator.applyRootMotion = value;
        }

        private void OnEnable()
        {
            animancer.Play(animationData.Idle);
            animationData.JumpStart.Events.OnEnd += () => animancer.Play(animationData.JumpLoop);
        }

        public void SetMoveSpeed(float speed)
        {
            animationData.Walk.State.Parameter = speed;
        }
        

        public void Move()
        {
            animancer.Play(animationData.Walk);
        }

        public void Idle()
        {
            animancer.Play(animationData.Idle);
        }

        public void Fall()
        {
            animancer.Play(animationData.JumpLoop);
        }

        public void BeginJump()
        {
            animancer.Play(animationData.JumpStart);
        }

        public void Attack(ClipTransition transition)
        {
            animancer.Play(transition);
        }

        public float GetCurrentDuration()
        {
            return animancer.States.Current.Duration;
        }
    }
}