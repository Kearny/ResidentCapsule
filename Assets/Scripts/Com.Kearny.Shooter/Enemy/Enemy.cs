using System.Collections;
using System.Collections.Generic;
using Com.Kearny.Shooter.GameMechanics;
using Unity.Burst;
using UnityEngine;
using UnityEngine.AI;
namespace Com.Kearny.Shooter.Enemy
{
    [BurstCompile]
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

        public GameObject deathEffect;

        private ParticleSystem _deathEffectParticleSystem;
        private NavMeshAgent _pathFinder;
        private Transform _target;
        private LivingEntity _targetEntity;
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
            _deathEffectParticleSystem = deathEffect.GetComponent<ParticleSystem>();
            _pathFinder = GetComponent<NavMeshAgent>();
            if (!GameObject.FindGameObjectWithTag("Player")) return;
            _currentState = State.Chasing;
            AcquireTarget();
            _targetEntity = _target.GetComponent<LivingEntity>();
            Debug.Log(_targetEntity.name);
            _targetEntity.OnDeath += OnTargetDeath;
            _myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            _targetCollisionRadius = _target.GetComponent<CapsuleCollider>().radius;
            StartCoroutine(UpdatePath());
        }

        private void AcquireTarget()
        {
            var targetTransforms = new List<Transform> { GameObject.FindGameObjectWithTag("Player").transform };
            foreach (var targetObject in GameObject.FindGameObjectsWithTag("Target"))
            {
                targetTransforms.Add(targetObject.transform);
            }
            _target = FindClosestTarget(targetTransforms);
            _hasTarget = _target != null;
        }

        private Transform FindClosestTarget(IEnumerable<Transform> targetTransforms)
        {
            Transform closest = null;
            var distance = Mathf.Infinity;
            Vector3 position = transform.position;
            foreach (var targetTransform in targetTransforms)
            {
                var distanceDiff = targetTransform.position - position;
                var currentDistance = distanceDiff.sqrMagnitude;
                if (currentDistance < distance)
                {
                    closest = targetTransform;
                    distance = currentDistance;
                }
            }
            return closest;
        }

        private void Update()
        {
            AcquireTarget();
            if (!_hasTarget) return;
            Debug.Log(_targetEntity.name);
            if (!(Time.time > _nextAttackTime)) return;
            var squareDistanceToTarget = (_target.position - transform.position).sqrMagnitude;
            // If can Attack
            if (squareDistanceToTarget < Mathf.Pow(
                AttackDistanceThreshold + _myCollisionRadius + _targetCollisionRadius,
                2
            ))
            {
                _nextAttackTime = Time.time + TimeBetweenAttacks;
                StartCoroutine(Attack());
            }
        }
        public override void TakeHit(float damage, Vector3 hitLocation, Vector3 hitDirection)
        {
            if (damage >= health)
            {
                Destroy(
                    Instantiate(deathEffect, hitLocation, Quaternion.FromToRotation(Vector3.forward, hitDirection)),
                    _deathEffectParticleSystem.main.duration
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
            var hasAppliedDamage = false;
            while (percent <= 1)
            {
                if (percent <= .8f && !hasAppliedDamage)
                {
                    hasAppliedDamage = true;
                    Debug.Log("Le bot tape sur : " + _targetEntity.name);
                    _targetEntity.TakeDamage(Damage);
                }
                percent += Time.deltaTime * attackSpeed;
                var interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
                transform.position = Vector3.Lerp(originalPosition, attackPosition, interpolation);
                yield return null;
            }
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