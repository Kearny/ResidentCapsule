﻿using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Com.Kearny.Shooter.GameMechanics
{
    [BurstCompile]
    public class GameManager : MonoBehaviour
    {
        private LinkedList<GameObject> _shuffledSpawners;

        private void Start()
        {
            _shuffledSpawners = Shuffle(GameObject.FindGameObjectsWithTag("EnemySpawn"));
        }
        
        public Transform GetRandomOpenSpawner()
        {
            var openSpawner = _shuffledSpawners.First();

            if (_shuffledSpawners.Count <= 1) return openSpawner.transform;
            
            // Moves the first select element to the end of the list
            _shuffledSpawners.AddLast(openSpawner);
            _shuffledSpawners.RemoveFirst();

            return openSpawner.transform;
        }

        private static LinkedList<GameObject> Shuffle(IList<GameObject> gameObjects)
        {
            for (var i = 0; i < gameObjects.Count; i++)
            {
                var tmp = gameObjects[i];
                var range = Random.Range(i, gameObjects.Count);
                gameObjects[i] = gameObjects[range];
                gameObjects[range] = tmp;
            }

            return new LinkedList<GameObject>(gameObjects);
        }
    }
}