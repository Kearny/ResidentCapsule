using Com.Kearny.Shooter.GameMechanics;
using UnityEngine;

namespace Com.Kearny.Shooter.Guns
{
    public class Projectile : MonoBehaviour
    {
        public LayerMask collisionMask;

        private float _speed;

        private const float Damage = 1;

        private const float Lifetime = 3;

        private const float SkinWidth = .1f;

        private void Start()
        {
            Destroy(gameObject, Lifetime);

            var initialCollisions = Physics.OverlapSphere(transform.position, .1f, collisionMask);
            if (initialCollisions.Length > 0)
            {
                OnHitObject(initialCollisions[0], transform.position);
            }
        }

        public void SetSpeed(float newSpeed)
        {
            _speed = newSpeed;
        }

        // Update is called once per frame
        private void Update()
        {
            var moveDistance = (Time.deltaTime * _speed);
            CheckCollisions(moveDistance);
            transform.Translate(Vector3.forward * moveDistance);
        }

        private void CheckCollisions(float moveDistance)
        {
            var localTransform = transform;
            var ray = new Ray(localTransform.position, localTransform.forward);
            if (Physics.Raycast(ray, out var hit, moveDistance + SkinWidth, collisionMask, QueryTriggerInteraction.Collide))
            {
                OnHitObject(hit.collider, hit.point);
            }
        }

        private void OnHitObject(Component colliderComponent, Vector3 hitPoint)
        {
            colliderComponent.GetComponent<IDamageable>()?.TakeHit(Damage, hitPoint, transform.forward);

            Destroy(gameObject);
        }
    }
}