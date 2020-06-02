using Unity.Burst;
using UnityEngine;

namespace Com.Kearny.Shooter.GameMechanics
{
    [BurstCompile]
    public class LivingEntity : MonoBehaviour, IDamageable
    {
        public float startingHealth;
        protected float health;
        protected bool isDead;

        public event System.Action OnDeath;

        protected virtual void Start()
        {
            health = startingHealth;
        }

        public virtual void TakeHit(float damage, Vector3 hitLocation, Vector3 hitDirection)
        {
            TakeDamage(damage);
        }

        public void TakeDamage(float damage)
        {
            Debug.Log("I'm " + name + " and I have " + health + " HP. I'm taking " + damage);
            health -= damage;

            Debug.Log("I'm " + name + " and I now have " + health + " HP.");

            if (health <= 0 && !isDead)
            {
                Die();
            }
        }

        private void Die()
        {
            isDead = true;
            OnDeath?.Invoke();
            Destroy(gameObject);
        }
    }
}