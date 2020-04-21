using UnityEngine;

namespace Com.Kearny.Shooter.Guns
{
    public class GunController : MonoBehaviour
    {
        public Transform weaponHold;
        public Gun equippedGun;
        private bool _isGunEquipped;

        private void Start()
        {
            if (equippedGun != null)
            {
                EquipGun(equippedGun);
            }
        }

        private void Update()
        {
            if (!_isGunEquipped) return;

            if (Input.GetButton("Fire1"))
            {
                OnTriggerHold();
            }

            if (Input.GetButtonUp("Fire1"))
            {
                OnTriggerRelease();
            }
        }

        public void EquipGun(Gun gunToEquip)
        {
            if (equippedGun != null)
            {
                Destroy(equippedGun.gameObject);
            }

            equippedGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation, weaponHold);
            _isGunEquipped = true;
        }

        private void OnTriggerHold()
        {
            equippedGun.OnTriggerHold();
        }

        private void OnTriggerRelease()
        {
            equippedGun.OnTriggerRelease();
        }
    }
}