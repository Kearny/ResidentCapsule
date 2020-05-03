using UnityEngine;

namespace Com.Kearny.Shooter.Weapons
{
    public abstract class Gun : MonoBehaviour
    {
        protected abstract FireMode FireMode { get; set; }

        // EDITOR
        [Header("References")]
        public Transform muzzle;
        public Projectile projectile;
        
        public Transform shell;
        public Transform shellEjection;
        
        [Header("Gun properties")]
        public float msBetweenShots = 100;
        public float muzzleVelocity = 35;

        public float recoilForce = 1;
        
        // PUBLIC
        
        // PRIVATE
        private const int BurstCount = 3;
        
        private MuzzleFlash _muzzleFlash;
        private float _nextShotTime;
        private bool _triggerReleasedSinceLastShot = true;
        private int _shotsRemainingInBurst;

        private void Start()
        {
            _muzzleFlash = GetComponent<MuzzleFlash>();
            _shotsRemainingInBurst = BurstCount;
        }

        private bool Shoot()
        {
            if (!(Time.time >= _nextShotTime)) return false;
            
            if (FireMode == FireMode.Burst && _shotsRemainingInBurst == 0)
            {
                return false;
            }

            switch (FireMode)
            {
                case FireMode.Burst:
                    _shotsRemainingInBurst--;
                    break;
                case FireMode.SemiAuto when !_triggerReleasedSinceLastShot:
                    return false;
            }

            _nextShotTime = Time.time + msBetweenShots / 1000;
            var newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
            newProjectile.SetSpeed(muzzleVelocity);

            Instantiate(shell, shellEjection.position, shellEjection.rotation);

            _muzzleFlash.Activate();

            // Weapon has fired
            return true;
        }

        public bool OnTriggerHold()
        {
            var hasFired = Shoot();
            _triggerReleasedSinceLastShot = false;

            return hasFired;
        }

        public void OnTriggerRelease()
        {
            _triggerReleasedSinceLastShot = true;
            _shotsRemainingInBurst = BurstCount;
        }
    }
}