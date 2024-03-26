using System;
using UnityEngine;

namespace NetDive.Test
{
    public class DeltaTimeTester : MonoBehaviour
    {
        private void Update()
        {
            Debug.Log($"In Update: {Time.deltaTime}");
        }

        private void FixedUpdate()
        {
            Debug.Log($"In FixedUpdate: {Time.deltaTime}");
        }
    }
}