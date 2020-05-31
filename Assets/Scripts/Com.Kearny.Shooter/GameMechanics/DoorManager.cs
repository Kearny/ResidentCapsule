using UnityEngine;
namespace Com.Kearny.Shooter.GameMechanics
{
    public class DoorManager : MonoBehaviour
    {
        private enum DoorState
        {
            OPENED,
            CLOSED,
            BROKEN
        }

        public float speed = 1;
        public float openDistance = 5;

        private DoorState _doorState = DoorState.CLOSED;
        private Vector3 _worldOpenPosition;
        private Vector3 _openPosition;
        private Vector3 _closedPosition;
        private Transform _playerTransform;

        // Start is called before the first frame update
        void Start()
        {
            _closedPosition = transform.position;
            _openPosition = new Vector3((transform.position.x - transform.localScale.x), transform.position.y, transform.position.z);
        }

        // Update is called once per frame
        void Update()
        {
            var players = GameObject.FindGameObjectsWithTag("Player");
            _playerTransform = players[0].transform;
            
            if (_doorState == DoorState.BROKEN)
            {
                return;
            }

            if (PlayerInReach() && _doorState == DoorState.CLOSED)
            {
                Open();
            }
            else if (!PlayerInReach() && _doorState != DoorState.CLOSED)
            {
                Debug.Log(PlayerInReach());
                Close();
            }
        }

        private bool PlayerInReach()
        {
            var distance = Vector3.Distance(_closedPosition, _playerTransform.position);
            Debug.Log(distance < openDistance);

            return distance < openDistance;
        }

        private void Open()
        {
            Debug.Log("Opening the door");
            while (transform.position != _openPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, _openPosition, Time.deltaTime * speed);
            }
            _doorState = DoorState.OPENED;
        }

        private void Close()
        {
            Debug.Log("Closing the door");
            while (transform.position != _closedPosition)
            {
                transform.position = Vector3.MoveTowards(transform.position, _closedPosition, Time.deltaTime * speed);
            }
            _doorState = DoorState.CLOSED;
        }
    }
}