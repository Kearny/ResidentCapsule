using System;
using UnityEngine;

namespace Com.Kearny.Shooter.Weapons
{
    public class PlayerWeaponsManager : MonoBehaviour
    {
        // EDITOR
        [Tooltip("Weapon the player will start with")]
        public Gun startingGun;

        [Header("References")] [Tooltip("Parent transform where all weapon will be added in the hierarchy")]
        public Transform weaponParentSocket;

        [Tooltip("Position for weapons when active but not actively aiming")]
        public Transform defaultWeaponPosition;

        [Tooltip("Position for weapons when aiming")]
        public Transform aimingWeaponPosition;

        [Tooltip("Position for inactive weapons")]
        public Transform downWeaponPosition;

        [Header("Misc")] [Tooltip("Speed at which the aiming animation is played")]
        public float aimingAnimationSpeed = 10f;

        [Tooltip("Layer to set FPS weapon gameObjects to")]
        public LayerMask FPSWeaponLayer;

        // PUBLIC
        public float BaseFov { get; private set; }

        // PRIVATE
        private const float AimZoomRatio = 0.5f;

        private Player.PlayerCharacterController _playerCharacterController;
        private Gun _equippedGun;
        private bool _isGunEquipped;
        private Vector3 _weaponMainLocalPosition;
        private bool _isAiming;

        private void Start()
        {
            _playerCharacterController = GetComponent<Player.PlayerCharacterController>();
            BaseFov = _playerCharacterController.mainCamera.fieldOfView;

            if (startingGun != null)
            {
                EquipGun(startingGun);
            }
        }

        private void Update()
        {
            if (!_isGunEquipped) return;

            // Handle aiming down sights
            if (Input.GetButtonDown("Fire2"))
            {
                _isAiming = !_isAiming;
            }

            // Handle shooting
            if (Input.GetButton("Fire1"))
            {
                OnTriggerHold();
            }

            if (Input.GetButtonUp("Fire1"))
            {
                OnTriggerRelease();
            }
        }

        private void LateUpdate()
        {
            UpdateWeaponAiming();
            // TODO : UpdateWeaponBob();
            // TODO : UpdateWeaponRecoil();

            // Set final weapon socket position based on all the combined animation influences
            weaponParentSocket.localPosition = _weaponMainLocalPosition;
            // TODO : weaponParentSocket.localPosition = _weaponMainLocalPosition + _weaponBobLocalPosition + _weaponRecoilLocalPosition;
        }

        private void UpdateWeaponAiming()
        {
            var weaponCameraFieldOfView = _playerCharacterController.mainCamera.fieldOfView;

            if (_isAiming)
            {
                _weaponMainLocalPosition = Vector3.Lerp(_weaponMainLocalPosition,
                    aimingWeaponPosition.localPosition,
                    aimingAnimationSpeed * Time.deltaTime);

                _playerCharacterController.SetFov(Mathf.Lerp(weaponCameraFieldOfView,
                    AimZoomRatio * BaseFov,
                    aimingAnimationSpeed * Time.deltaTime));
            }
            else
            {
                _weaponMainLocalPosition = Vector3.Lerp(_weaponMainLocalPosition,
                    defaultWeaponPosition.localPosition,
                    aimingAnimationSpeed * Time.deltaTime);

                _playerCharacterController.SetFov(Mathf.Lerp(weaponCameraFieldOfView,
                    BaseFov,
                    aimingAnimationSpeed * Time.deltaTime));
            }
        }

        private void EquipGun(Gun gunToEquip)
        {
            if (_equippedGun != null)
            {
                Destroy(_equippedGun.gameObject);
            }

            _weaponMainLocalPosition = defaultWeaponPosition.localPosition;

            _equippedGun = Instantiate(gunToEquip, weaponParentSocket);
            _isGunEquipped = true;
        }

        private void OnTriggerHold()
        {
            _equippedGun.OnTriggerHold();
        }

        private void OnTriggerRelease()
        {
            _equippedGun.OnTriggerRelease();
        }
    }
}