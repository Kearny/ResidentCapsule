using UnityEngine;

namespace Com.Kearny.Shooter.Guns
{
    public abstract class Gun : MonoBehaviour
    {
        protected abstract FireMode FireMode { get; set; }

        public Transform muzzle;
        public Projectile projectile;
        public float msBetweenShots = 100;
        public float muzzleVelocity = 35;

        public Transform shell;
        public Transform shellEjection;

        private MuzzleFlash _muzzleFlash;

        private float _nextShotTime;

        private bool _triggerReleasedSinceLastShot = true;
        private int _shotsRemainingInBurst;
        
        private const int BurstCount = 3;

        private void Start()
        {
            _muzzleFlash = GetComponent<MuzzleFlash>();
            _shotsRemainingInBurst = BurstCount;
        }

        private void Shoot()
        {
            if (!(Time.time >= _nextShotTime)) return;
            
            if (FireMode == FireMode.Burst && _shotsRemainingInBurst == 0)
            {
                return;
            }

            switch (FireMode)
            {
                case FireMode.Burst:
                    _shotsRemainingInBurst--;
                    break;
                case FireMode.SemiAuto when !_triggerReleasedSinceLastShot:
                    return;
            }

            _nextShotTime = Time.time + msBetweenShots / 1000;
            var newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation);
            newProjectile.SetSpeed(muzzleVelocity);

            Instantiate(shell, shellEjection.position, shellEjection.rotation);

            _muzzleFlash.Activate();
        }

        public void OnTriggerHold()
        {
            Shoot();
            _triggerReleasedSinceLastShot = false;
        }

        public void OnTriggerRelease()
        {
            _triggerReleasedSinceLastShot = true;
            _shotsRemainingInBurst = BurstCount;
        }
    }
}