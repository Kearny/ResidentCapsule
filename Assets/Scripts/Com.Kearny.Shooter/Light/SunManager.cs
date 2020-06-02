using UnityEngine;

namespace Com.Kearny.Shooter.Light
{
    public class SunManager : MonoBehaviour
    {
        [Header("Sun")] [Tooltip("Enable the sun rotation")]
        public bool isRotating;

        [Tooltip("The speed at which sun is rotating (0.1 equals a complete rotation in 1h)")]
        public float rotationSpeed;

        // Update is called once per frame
        private void Update()
        {
            if (isRotating)
            {
                transform.Rotate(Vector3.right * (rotationSpeed * Time.deltaTime));
            }
        }
    }
}