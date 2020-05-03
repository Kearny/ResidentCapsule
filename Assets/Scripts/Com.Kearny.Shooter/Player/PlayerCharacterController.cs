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

        [Tooltip("Force at which player jump")]
        public float jumpForce = 10;

        [Tooltip("Gravity force")] public float gravity = 40f;

        // PUBLIC
        public CharacterController CharacterController { get; private set; }

        public Vector3 moveDirection;

        // PRIVATE
        private const float SprintFovModifier = 1.15f;
        private readonly Quaternion _camCenter = new Quaternion(0, 0, 0, 1);
        private Transform _mainCameraTransform;
        private bool _isRunning;
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

            CharacterController = GetComponent<CharacterController>();
            _playerWeaponsManager = GetComponent<PlayerWeaponsManager>();
        }

        private void Update()
        {
            SetX();
            SetY();

            if (CharacterController.isGrounded)
            {
                // We are grounded, so recalculate
                // move direction directly from axes

                var verticalInput = Input.GetAxisRaw("Vertical");
                var horizontalInput = Input.GetAxis("Horizontal");

                var localTransform = transform;
                var forwardMovement = localTransform.forward * verticalInput;
                var rightMovement = localTransform.right * horizontalInput;
                moveDirection = Vector3.ClampMagnitude(forwardMovement + rightMovement, 1f);

                if (Input.GetButton("Jump"))
                {
                    moveDirection.y = jumpForce;
                }

                // Sprinting
                _isRunning = Input.GetKey(KeyCode.LeftShift) && !(verticalInput < 0f);

                // Change camera FOV base on running state
                if (_isRunning)
                {
                    SetFov(Mathf.Lerp(mainCamera.fieldOfView, _playerWeaponsManager.BaseFov * SprintFovModifier,
                        Time.deltaTime * 5f));

                    moveDirection *= moveSpeed * 2;
                }
                else
                {
                    SetFov(Mathf.Lerp(mainCamera.fieldOfView, _playerWeaponsManager.BaseFov, Time.deltaTime * 5f));
                    moveDirection *= moveSpeed;
                }
            }

            // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
            // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
            // as an acceleration (ms^-2)
            moveDirection.y -= gravity * Time.deltaTime;

            // Move the controller
            CharacterController.Move(moveDirection * Time.deltaTime);
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

        private void Rotate(Quaternion delta)
        {
            transform.localRotation = delta;
        }
    }
}