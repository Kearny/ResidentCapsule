using System.Collections;
using UnityEngine;

namespace Com.Kearny.Shooter.GameMechanics
{
    public class Spawner : MonoBehaviour
    {
        public Wave[] waves;
        public Enemy.Enemy enemy;

        public event System.Action<int> OnNewWave;

        private Wave _currentWave;
        private int _currentWaveNumber;

        private int _enemiesRemainingToSpawn;
        private int _enemiesRemainingAlive;
        private float _nextSpawnTime;

        private MapController _mapController;

        private void Start()
        {
            _mapController = FindObjectOfType<MapController>();
            NextWave();
        }

        private void Update()
        {
            if (enemy == null) return;
            if (_enemiesRemainingToSpawn <= 0 || !(Time.time > _nextSpawnTime)) return;

            _enemiesRemainingToSpawn--;
            _nextSpawnTime = Time.time + _currentWave.timeBetweenSpawns;

            StartCoroutine(SpawnEnemy());
        }

        private IEnumerator SpawnEnemy()
        {
            const float spawnDelayBeforeFirstSpawn = 1;

            Transform randomSpawner = _mapController.GetRandomOpenSpawner();

            float spawnTimer = 0;
            while (spawnTimer < spawnDelayBeforeFirstSpawn)
            {
                spawnTimer += Time.deltaTime;
                yield return null;
            }

            var spawnedEnemy = Instantiate(enemy, randomSpawner.position, Quaternion.identity);
            spawnedEnemy.OnDeath += OnEnemyDeath;
        }

        private void OnEnemyDeath()
        {
            _enemiesRemainingAlive--;

            if (_enemiesRemainingAlive == 0)
            {
                NextWave();
            }
        }

        private void NextWave()
        {
            _currentWaveNumber++;

            if (_currentWaveNumber - 1 >= waves.Length) return;

            _currentWave = waves[_currentWaveNumber - 1];

            _enemiesRemainingToSpawn = _currentWave.enemyCount;
            _enemiesRemainingAlive = _enemiesRemainingToSpawn;

            OnNewWave?.Invoke(_currentWaveNumber);
        }

        [System.Serializable]
        public class Wave
        {
            public int enemyCount;
            public float timeBetweenSpawns;
        }
    }
}