using UnityEngine;

namespace Com.Kearny.Shooter.GameMechanics
{
    public interface IDamageable
    {
        void TakeHit(float damage, Vector3 hitLocation, Vector3 hitDirection);
    }
}