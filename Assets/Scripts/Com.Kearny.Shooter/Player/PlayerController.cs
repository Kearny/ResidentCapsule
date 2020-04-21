using UnityEngine;

namespace Com.Kearny.Shooter.Player
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        private Vector3 _velocity;
        private CharacterController _characterController;

        // Start is called before the first frame update
        private void Start()
        {
            _characterController = GetComponent<CharacterController>();
        }

        public void Move(Vector3 moveVelocity)
        {
            _velocity = _characterController.isGrounded ? moveVelocity : Vector3.zero;
        }

        private void FixedUpdate()
        {
            _characterController.SimpleMove(_velocity);
        }

        public void Rotate(Quaternion delta)
        {
            transform.localRotation = delta;
        }
    }
}