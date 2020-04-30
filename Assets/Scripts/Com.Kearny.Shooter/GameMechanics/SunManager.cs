﻿using UnityEngine;

namespace Com.Kearny.Shooter.GameMechanics
{
    public class SunManager : MonoBehaviour
    {
        [Tooltip("The speed at which sun is rotating (0.1 equals a complete rotation in 1h)")]
        public float rotationSpeed;

        // Update is called once per frame
        private void Update()
        {
            transform.Rotate(Vector3.right * (rotationSpeed * Time.deltaTime));
        }
    }
}
