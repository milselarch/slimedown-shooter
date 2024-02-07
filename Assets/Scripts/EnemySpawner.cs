using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour {
    // Start is called before the first frame update
    public Tilemap tileMap;
    public float spawnRadius = 5.0f;
    public GameObject slimePrefab;
    public GameObject bombSlimePrefab;
    public float spawnInterval = 0.2f;
    
    public IntVariable waveTimestamp;
    public IntVariable waveCounter;
    public UnityEvent waveUpdate;
    // event to drain players health when time for wave is up
    public UnityEvent<int> timerDrainHealth;

    private Vector3 _startPosition;
    private bool _destroyed = false;
    private int _enemiesKilled = 0;
    [CanBeNull] private IEnumerator _spawnCoroutine;

    private void Start() {
        _startPosition = transform.position;
        StartCoroutine(TimerUpdateLoop());
        GameRestart();
    }

    public void GameRestart() {
        _enemiesKilled = 0;
        waveCounter.SetValue(0);
        waveTimestamp.SetValue(0);

        if (_spawnCoroutine != null) {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
        _spawnCoroutine = SpawnAttackWave();
        StartCoroutine(_spawnCoroutine);
    }

    private IEnumerator TimerUpdateLoop() {
        /*
         * Drains player health every 3 seconds if hes
         * ran out of time to complete the current attack wave
         */
        while (!_destroyed) {
            var timestampNow = (int) Time.time;
            var allowedWaveDuration = 20 + 5 * waveCounter.Value;
            var durationPassed = timestampNow - waveTimestamp.Value;
            var durationLeft = allowedWaveDuration - durationPassed;

            if (durationLeft <= -1) {
                var penalty = (int) Math.Log10(-durationLeft) + 1;
                timerDrainHealth.Invoke(penalty);
            }
            
            yield return new WaitForSeconds(3.0f);
        }

        yield return null;
    }

    public void OnEnemyKilled() {
        _enemiesKilled += 1;

        // check if any enemies from the previous wave are still alive
        if (GameState.HasLivingEnemies() || GameState.spawning) {
            return;
        }

        Debug.Log("WAVE IS OVER");
        StartCoroutine(SpawnAttackWave());
        waveUpdate.Invoke();
    }

    private IEnumerator SpawnAttackWave() {
        /*
         * spawn slimes in an attack wave
         */
        Assert.IsFalse(GameState.spawning);
        
        _enemiesKilled = 0;
        waveCounter.Increment();
        var currentWave = waveCounter.Value;
        var slimesToSpawn = currentWave * 2;
        
        GameState.ResetWaveSpawnFlags();
        Assert.IsTrue(GameState.spawning);
        yield return null;

        // set start of attack wave to right now
        waveTimestamp.SetValue((int) Time.time);
        var firstSpawn = true;
        
        while (GameState.spawned < slimesToSpawn) {
            if (!firstSpawn) {
                yield return new WaitForSeconds(spawnInterval);
            } else {
                firstSpawn = false;
            }
            
            /*
             * Note to self: made sure that there are no yields between
             * the last enemy instantiation and the function exit, so that theres
             * a 0% chance that the last enemy is killed and a new wave is spawned
             * before the current spawning code exits
             */

            var offsetX = 2 * Random.Range(0.0f, spawnRadius) - spawnRadius;
            var offsetY = 2 * Random.Range(0.0f, spawnRadius) - spawnRadius;
            var spawnPosition = new Vector3(
                _startPosition.x + offsetX,
                _startPosition.y + offsetY,
                _startPosition.z
            );

            var spawnable = InTileMap(spawnPosition);
            if (!spawnable) { continue; }

            var enemy = SpawnEnemy(spawnPosition);

            enemy.transform.SetParent(transform, true);
            var enemyControllable = enemy.GetComponent<IBaseEnemyControllerable>();
            GameState.AddLivingEnemy(enemyControllable.GetBaseController());
        }

        GameState.StopSpawning();
        Assert.IsFalse(GameState.spawning);
    }


    private GameObject SpawnEnemy(Vector3 spawnPosition) {
        var enemy = Instantiate(
            slimePrefab, spawnPosition,
            Quaternion.identity
        );

        return enemy;
    }
    
    private bool InTileMap(Vector3 position) {
        // check if the spawn position is in spawnable tile area
        var cellPosition = tileMap.WorldToCell(position);
        return tileMap.HasTile(cellPosition);
    }

    private void OnDestroy() {
        _destroyed = true;
    }
}
