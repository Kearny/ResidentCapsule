using System.Collections;
using UnityEngine;

namespace Com.Kearny.Shooter.GameMechanics
{
    [RequireComponent(typeof(Enemy.Enemy))]
    public class Spawner : MonoBehaviour
    {
        public Wave wave;
        public Enemy.Enemy enemy;

        public event System.Action<int> OnNewWave;

        private int _enemiesRemainingAlive = 0;
        private float _nextSpawnTime;

        private MapController _mapController;


        private void Start()
        {
            _mapController = FindObjectOfType<MapController>();
        }

        private void Update()
        {
            if (_enemiesRemainingAlive > wave.enemyCount || !(Time.time > _nextSpawnTime)) return;

            _nextSpawnTime = Time.time + wave.timeBetweenSpawns;

            StartCoroutine(SpawnEnemy());
        }

        private IEnumerator SpawnEnemy()
        {
            const float spawnDelayBeforeFirstSpawn = 1;

            var randomSpawner = _mapController.GetRandomOpenSpawner();

            float spawnTimer = 0;
            while (spawnTimer < spawnDelayBeforeFirstSpawn)
            {
                spawnTimer += Time.deltaTime;
                yield return null;
            }

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