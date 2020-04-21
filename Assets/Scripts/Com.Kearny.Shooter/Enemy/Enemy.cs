using System.Collections;
using Com.Kearny.Shooter.GameMechanics;
using UnityEngine;
using UnityEngine.AI;

namespace Com.Kearny.Shooter.Enemy
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Enemy : LivingEntity
    {
        private enum State
        {
            Idle,

            Chasing,

            Attacking
        };
        private State _currentState;

        public ParticleSystem deathEffect;

        private NavMeshAgent _pathFinder;
        private Transform _target;
        private LivingEntity _targetEntity;
        private Material _skinMaterial;

        private Color _originalColor;
        private const float AttackDistanceThreshold = 1.5f;
        private const float TimeBetweenAttacks = 1;
        private const float Damage = 1;
        private float _nextAttackTime;
        private float _myCollisionRadius;
        private float _targetCollisionRadius;
        private bool _hasTarget;

        protected override void Start()
        {
            base.Start();

            _pathFinder = GetComponent<NavMeshAgent>();
            _skinMaterial = GetComponent<Renderer>().material;
            _originalColor = _skinMaterial.color;

            if (!GameObject.FindGameObjectWithTag("Player")) return;
            _currentState = State.Chasing;
            _hasTarget = true;

            _target = GameObject.FindGameObjectWithTag("Player").transform;
            _targetEntity = _target.GetComponent<LivingEntity>();
            _targetEntity.OnDeath += OnTargetDeath;

            _myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            _targetCollisionRadius = _target.GetComponent<CapsuleCollider>().radius;

            StartCoroutine(UpdatePath());
        }

        private void Update()
        {
            if (!_hasTarget) return;
            if (!(Time.time > _nextAttackTime)) return;

            var squareDistanceToTarget = (_target.position - transform.position).sqrMagnitude;
            if (!(squareDistanceToTarget < Mathf.Pow(
                AttackDistanceThreshold + _myCollisionRadius + _targetCollisionRadius,
                2
            ))) return;

            _nextAttackTime = Time.time + TimeBetweenAttacks;
            StartCoroutine(Attack());
        }

        public override void TakeHit(float damage, Vector3 hitLocation, Vector3 hitDirection)
        {
            if (damage >= health)
            {
                Destroy(
                    Instantiate(deathEffect, hitLocation, Quaternion.FromToRotation(Vector3.forward, hitDirection)),
                    deathEffect.main.duration
                );
            }

            base.TakeHit(damage, hitLocation, hitDirection);
        }

        private void OnTargetDeath()
        {
            _hasTarget = false;
            _currentState = State.Idle;
        }

        private IEnumerator Attack()
        {
            _currentState = State.Attacking;

            var transformPosition = transform.position;
            var originalPosition = transformPosition;

            var targetPosition = _target.position;
            var directionToTarget = (targetPosition - transformPosition).normalized;
            var attackPosition = targetPosition - directionToTarget;

            const float attackSpeed = 3;
            float percent = 0;

            _skinMaterial.color = Color.red;
            var hasAppliedDamage = false;

            while (percent <= 1)
            {
                if (percent <= .8f && !hasAppliedDamage)
                {
                    hasAppliedDamage = true;
                    _targetEntity.TakeDamage(Damage);
                }

                percent += Time.deltaTime * attackSpeed;
                var interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
                transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);

                yield return null;
            }

            _skinMaterial.color = _originalColor;

            _currentState = State.Chasing;
        }

        private IEnumerator UpdatePath()
        {
            const float refreshRate = 0.5f;

            while (_hasTarget)
            {
                if (!isDead && _currentState == State.Chasing)
                {
                    var targetPosition = _target.position;
                    var directionToTarget = (targetPosition - transform.position).normalized;
                    var targetedPosition =
                        targetPosition - directionToTarget *
                        (_myCollisionRadius + _targetCollisionRadius + AttackDistanceThreshold / 2);
                    _pathFinder.SetDestination(targetedPosition);
                }

                yield return new WaitForSeconds(refreshRate);
            }
        }
    }
}