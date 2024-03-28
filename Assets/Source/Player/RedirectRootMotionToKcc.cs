using Animancer;
using KinematicCharacterController;
using UnityEngine;

namespace NetDive.Player
{
    public class RedirectRootMotionToKcc : RedirectRootMotion<KinematicCharacterMotor>
    {
        protected override void OnAnimatorMove()
        {
            if (!ApplyRootMotion)
                return;

            Target.MoveCharacter(Target.TransientPosition + Animator.deltaPosition + Target.AttachedRigidbodyVelocity * Time.deltaTime);
            //Debug.Log(Target.TransientPosition +);
        }
    }
}