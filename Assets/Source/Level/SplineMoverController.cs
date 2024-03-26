using System;
using KinematicCharacterController;
using UnityEngine;
using UnityEngine.Splines;

namespace NetDive.Level
{
    public class SplineMoverController : MonoBehaviour, IMoverController
    {
        private PhysicsMover _mover;
        public SplineAnimate SplineAnimate { get; set; }

        private void OnEnable()
        {
            _mover = GetComponent<PhysicsMover>();
            _mover.MoverController = this;

            SplineAnimate = GetComponent<SplineAnimate>();
        }

        public void UpdateMovement(out Vector3 goalPosition, out Quaternion goalRotation, float deltaTime)
        {
            SplineAnimate.Update();
            goalPosition = transform.position;
            goalRotation = transform.rotation;
        }
    }
}