using Com.Kearny.Shooter.GameMechanics;
using UnityEngine;

namespace Com.Kearny.Shooter.Enemy
{
    [RequireComponent(typeof(Shooter.Enemy.Enemy))]
    public class Spawner : MonoBehaviour
    {
        public Wave wave;
        public Shooter.Enemy.Enemy enemy;

        private int _enemiesRemainingAlive = 0;
        private float _nextSpawnTime;

        private GameManager _gameManager;


        private void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
        }

        private void Update()
        {
            if (_enemiesRemainingAlive >= wave.enemyCount) return;

            _nextSpawnTime = Time.time + wave.timeBetweenSpawns;

            SpawnEnemy();
        }

        private void SpawnEnemy()
        {
            var randomSpawner = _gameManager.GetRandomOpenSpawner();

            var spawnedEnemy = Instantiate(enemy, randomSpawner.position, Quaternion.identity);
            _enemiesRemainingAlive++;
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }

        private void OnEnemyDeath()
        {
            _enemiesRemainingAlive--;
        }

        [System.Serializable]
        public class Wave
        {
            public int enemyCount;
            public float timeBetweenSpawns;
        }
    }
}