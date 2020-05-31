using Com.Kearny.Shooter.GameMechanics;
using Unity.Burst;
using UnityEngine;

namespace Com.Kearny.Shooter.Enemy
{
    [BurstCompile]
    [RequireComponent(typeof(Enemy))]
    public class Spawner : MonoBehaviour
    {
        public Wave wave;
        public Enemy enemy;

        private int _enemiesRemainingAlive = 0;
        private float _nextSpawnTime;

        private GameManager _gameManager;


        private void Start()
        {
            _gameManager = FindObjectOfType<GameManager>();
        }

        private void Update()
        {
            if (_enemiesRemainingAlive < wave.enemyCount && Time.time > _nextSpawnTime)
            {
                _nextSpawnTime = Time.time + wave.timeBetweenSpawns;

                SpawnEnemy();
            }
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