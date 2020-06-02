using System;
using System.Collections;
using UnityEngine;

namespace Com.Kearny.Shooter.GameMechanics
{
    public class Door : LivingEntity
    {
        private enum DoorState
        {
            Opened,
            Closed
        }
        
        public float speed = 1;
        public float openDistance = 5;
        
        private DoorState _doorState = DoorState.Closed;
        private Vector3 _openPosition;
        private Vector3 _closedPosition;
        private Transform _playerTransform;
        
        protected override void Start()
        {
            base.Start();
            var transform1 = transform;
            var position = transform1.position;
            _closedPosition = position;
            _openPosition = new Vector3(position.x, position.y, (position.z - transform1.localScale.x));
            _playerTransform = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        }
        
        private void Update()
        {
            if (PlayerInReach() && _doorState == DoorState.Closed)
            {
                StartCoroutine(Open());
            }
            else if (!PlayerInReach() && _doorState != DoorState.Closed)
            {
                StartCoroutine(Close());
            }
        }
        
        private bool PlayerInReach()
        {
            var distance = Vector3.Distance(_closedPosition, _playerTransform.position);
            return distance < openDistance;
        }
        
        private IEnumerator Open()
        {
            while (transform.position != _openPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, _openPosition, Time.deltaTime * speed);
            }
            
            _doorState = DoorState.Opened;

            yield return null;
        }
        private IEnumerator Close()
        {
            while (transform.position != _closedPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, _closedPosition, Time.deltaTime * speed);
            }
            
            _doorState = DoorState.Closed;

            yield return null;
        }
    }
}