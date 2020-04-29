using System;
using Com.Kearny.Shooter.GameMechanics;
using Com.Kearny.Shooter.Weapons;
using UnityEngine;

namespace Com.Kearny.Shooter.Player
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(PlayerWeaponsManager))]
    public class PlayerCharacterController : LivingEntity
    {
        // EDITOR
        [Header("References")] [Tooltip("Reference to the main user camera")]
        public Camera mainCamera;

        [Header("Mouse input")] [Tooltip("Sensitivity on X axis")]
        public float xSensitivity;

        [Tooltip("Sensitivity on Y axis")] public float ySensitivity;

        [Tooltip("Max angle on X axis ")] public float maxAngle = 75f;

        [Header("Movements")] [Tooltip("Speed at which player moves")]
        public float moveSpeed = 5;

        // PUBLIC

        // PRIVATE
        private const float SprintFovModifier = 1.15f;
        private readonly Quaternion _camCenter = new Quaternion(0, 0, 0, 1);
        private Transform _mainCameraTransform;
        private bool _isRunning;
        private Vector3 _velocity;
        private CharacterController _characterController;
        private PlayerWeaponsManager _playerWeaponsManager;
        
        public void SetFov(float fov)
        {
            mainCamera.fieldOfView = fov;
        }

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();

            _mainCameraTransform = mainCamera.transform;

            _characterController = GetComponent<CharacterController>();
            _playerWeaponsManager = GetComponent<PlayerWeaponsManager>();
        }

        // Update is called once per frame
        private void Update()
        {
            // Body movement input
            var horizontalInput = Input.GetAxisRaw("Horizontal");
            var verticalInput = Input.GetAxisRaw("Vertical");

            var localTransform = transform;
            var forwardMovement = localTransform.forward * verticalInput;
            var rightMovement = localTransform.right * horizontalInput;
            Move(Vector3.ClampMagnitude(forwardMovement + rightMovement, 1f) * moveSpeed);

            SetX();
            SetY();

            // Sprinting
            _isRunning = Input.GetKey(KeyCode.LeftShift) && !(verticalInput < 0f);

            // Change camera FOV base on running state
            if (_isRunning)
            {
                SetFov(Mathf.Lerp(mainCamera.fieldOfView, _playerWeaponsManager.BaseFov * SprintFovModifier, Time.deltaTime * 5f));
            }
            else
            {
                SetFov(Mathf.Lerp(mainCamera.fieldOfView, _playerWeaponsManager.BaseFov, Time.deltaTime * 5f));
            }
        }

        private void SetY()
        {
            var yAngle = Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
            var xRotation = Quaternion.AngleAxis(yAngle, -Vector3.right);
            var delta = _mainCameraTransform.localRotation * xRotation;
            var angle = Quaternion.Angle(_camCenter, delta);

            if (!(angle > -maxAngle) || !(angle < maxAngle)) return;

            _mainCameraTransform.localRotation = delta;
        }

        private void SetX()
        {
            var yAngle = Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime;
            var xRotation = Quaternion.AngleAxis(yAngle, Vector3.up);
            var delta = transform.localRotation * xRotation;

            Rotate(delta);
        }

        private void Move(Vector3 moveVelocity)
        {
            if (_characterController.isGrounded && _isRunning)
            {
                moveVelocity *= 2;
            }

            _velocity = _characterController.isGrounded ? moveVelocity : Vector3.zero;
        }

        private void FixedUpdate()
        {
            _characterController.SimpleMove(_velocity);
        }

        private void Rotate(Quaternion delta)
        {
            transform.localRotation = delta;
        }
    }
}