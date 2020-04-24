﻿using Com.Kearny.Shooter.GameMechanics;
using UnityEngine;

namespace Com.Kearny.Shooter.Player
{
    [RequireComponent(typeof(PlayerController))]
    public class Player : LivingEntity
    {
        private Transform _mainCameraTransform;

        public float moveSpeed = 5;

        public float xSensitivity;
        public float ySensitivity;
        public float maxAngle = 75f;

        private PlayerController _controller;
        public Camera mainCamera;

        private readonly Quaternion _camCenter = new Quaternion(0, 0, 0, 1);

        private float _baseFov;
        private float _sprintFovModfier = 1.15f;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            
            _mainCameraTransform = mainCamera.transform;
            _controller = GetComponent<PlayerController>();
            _baseFov = mainCamera.fieldOfView;
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
            _controller.Move(Vector3.ClampMagnitude(forwardMovement + rightMovement, 1f) * moveSpeed);

            SetX();
            SetY();
            
            // Sprinting
            _controller.IsRunning = Input.GetKey(KeyCode.LeftShift) && !(verticalInput < 0f) ;
            if (_controller.IsRunning)
            {
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, _baseFov * _sprintFovModfier, Time.deltaTime * 5f);
            }
            else
            {
                mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, _baseFov, Time.deltaTime * 5f);
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

            _controller.Rotate(delta);
        }
    }
}