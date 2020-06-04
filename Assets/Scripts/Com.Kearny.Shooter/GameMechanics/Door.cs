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
        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            var transform1 = transform;
            var position = transform1.position;
            _closedPosition = position;
            _openPosition = new Vector3((position.x - transform1.localScale.x), position.y, position.z);
            _playerTransform = GameObject.FindGameObjectsWithTag("Player")[0].transform;
        }
        // Update is called once per frame
        private void Update()
        {
            if (PlayerInReach() && _doorState == DoorState.Closed)
            {
                Open();
            }
            else if (!PlayerInReach() && _doorState != DoorState.Closed)
            {
                Close();
            }
        }
        private bool PlayerInReach()
        {
            var distance = Vector3.Distance(_closedPosition, _playerTransform.position);
            return distance < openDistance;
        }
        private void Open()
        {
            while (transform.position != _openPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, _openPosition, Time.deltaTime * speed);
            }
            _doorState = DoorState.Opened;
        }
        private void Close()
        {
            while (transform.position != _closedPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, _closedPosition, Time.deltaTime * speed);
            }
            _doorState = DoorState.Closed;
        }
    }
}