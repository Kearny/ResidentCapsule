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

        [Header("Weapon Bob")]
        [Tooltip("Frequency at which the weapon will move around in the screen when the player is in movement")]
        public float bobFrequency = 10;

        [Tooltip("How fast the weapon bob is applied, the bigger value the fastest")]
        public float bobSharpness = 10f;

        [Tooltip("Distance the weapon bobs when not aiming")]
        public float defaultBobAmount = 0.05f;

        [Tooltip("Distance the weapon bobs when aiming")]
        public float aimingBobAmount = 0.02f;

        [Header("Weapon Recoil")]
        [Tooltip("This will affect how fast the recoil moves the weapon, the bigger the value, the fastest")]
        public float recoilSharpness = 50f;

        [Tooltip("Maximum distance the recoil can affect the weapon")]
        public float maxRecoilDistance = 0.5f;

        [Tooltip("How fast the weapon goes back to it's original position after the recoil is finished")]
        public float recoilRestitutionSharpness = 10f;

        // PUBLIC
        public float BaseFov { get; private set; }

        // PRIVATE
        private const float AimZoomRatio = 0.5f;

        private Player.PlayerCharacterController _playerCharacterController;
        private Gun _equippedGun;
        private bool _isGunEquipped;
        private Vector3 _weaponMainLocalPosition;
        private bool _isAiming;
        private float _weaponBobFactor;
        private Vector3 _weaponBobLocalPosition;
        private Vector3 _weaponRecoilLocalPosition;
        private Vector3 _accumulatedRecoil;

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
            bool hasFired = false;
            if (Input.GetButton("Fire1"))
            {
                hasFired = OnTriggerHold();
            }

            if (Input.GetButtonUp("Fire1"))
            {
                OnTriggerRelease();
            }

            // Handle accumulating recoil
            if (hasFired)
            {
                _accumulatedRecoil += Vector3.back * _equippedGun.recoilForce;
                _accumulatedRecoil = Vector3.ClampMagnitude(_accumulatedRecoil, maxRecoilDistance);
            }
        }

        private void LateUpdate()
        {
            UpdateWeaponAiming();
            UpdateWeaponBob();
            UpdateWeaponRecoil();

            // Set final weapon socket position based on all the combined animation influences
            weaponParentSocket.localPosition = _weaponMainLocalPosition + _weaponBobLocalPosition + _weaponRecoilLocalPosition;
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

        private void UpdateWeaponBob()
        {
            if (Time.deltaTime > 0)
            {
                var playerCharacterVelocity = _playerCharacterController.moveDirection;

                // calculate a smoothed weapon bob amount based on how close to our max grounded movement velocity we are
                float characterMovementFactor = 0;
                if (_playerCharacterController.CharacterController.isGrounded)
                {
                    characterMovementFactor = Mathf.Clamp01(
                        playerCharacterVelocity.magnitude /
                        (_playerCharacterController.moveSpeed * (_playerCharacterController.moveSpeed * 2))
                    );
                }

                _weaponBobFactor = Mathf.Lerp(_weaponBobFactor, characterMovementFactor, bobSharpness * Time.deltaTime);
                
                // Calculate vertical and horizontal weapon bob values based on a sine function
                var bobAmount = _isAiming ? aimingBobAmount : defaultBobAmount;
                var frequency = bobFrequency;
                var hBobValue = Mathf.Sin(Time.time * frequency) * bobAmount * _weaponBobFactor;
                var vBobValue = ((Mathf.Sin(Time.time * frequency * 2f) * 0.5f) + 0.5f) * bobAmount * _weaponBobFactor;
                
                // Apply weapon bob
                _weaponBobLocalPosition.x = hBobValue;
                _weaponBobLocalPosition.y = Mathf.Abs(vBobValue);
            }
        }

        private void UpdateWeaponRecoil()
        {
            // if the accumulated recoil is further away from the current position, make the current position move towards the recoil target
            if (_weaponRecoilLocalPosition.z >= _accumulatedRecoil.z * 0.99f)
            {
                _weaponRecoilLocalPosition = Vector3.Lerp(_weaponRecoilLocalPosition, 
                    _accumulatedRecoil, 
                    recoilSharpness * Time.deltaTime);
            }
            // otherwise, move recoil position to make it recover towards its resting pose
            else
            {
                _weaponRecoilLocalPosition = Vector3.Lerp(
                    _weaponRecoilLocalPosition,
                    Vector3.zero, 
                    recoilRestitutionSharpness * Time.deltaTime);
                _accumulatedRecoil = _weaponRecoilLocalPosition;
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

        private bool OnTriggerHold()
        {
            return _equippedGun.OnTriggerHold();
        }

        private void OnTriggerRelease()
        {
            _equippedGun.OnTriggerRelease();
        }
    }
}