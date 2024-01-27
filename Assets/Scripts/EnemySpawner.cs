using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour {
    // Start is called before the first frame update
    public Tilemap tileMap;
    public float spawnRadius = 5.0f;
    public GameObject enemyPrefab;
    public float spawnInterval = 0.2f;
    
    public IntVariable waveTimestamp;
    public IntVariable waveCounter;
    public UnityEvent waveUpdate;
    // event to drain players health when time for wave is up
    public UnityEvent<int> timerDrainHealth;

    private Vector3 _startPosition;
    private bool _destroyed = false;
    
    private int _lastSpawnedWave = 0;
    private int _enemiesKilled = 0;

    private void Start() {
        waveCounter.SetValue(0);
        waveTimestamp.SetValue(0);
        _startPosition = transform.position;
        StartCoroutine(SpawnAttackWave());
        StartCoroutine(TimerUpdateLoop());
    }

    private IEnumerator TimerUpdateLoop() {
        /*
         * Drains player health every 3 seconds if he's
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
        while (GameState.spawning) {
            // wait for previous wave to finish spawning
            yield return null;
        }
        
        _enemiesKilled = 0;
        waveCounter.Increment();
        GameState.ResetWave();

        // set start of attack wave to right now
        waveTimestamp.SetValue((int) Time.time);
        
        while (GameState.spawned < waveCounter.Value + 3) {
            var offsetX = 2 * Random.Range(0.0f, spawnRadius) - spawnRadius;
            var offsetY = 2 * Random.Range(0.0f, spawnRadius) - spawnRadius;
            var spawnPosition = new Vector3(
                _startPosition.x + offsetX,
                _startPosition.y + offsetY,
                _startPosition.z
            );

            var spawnable = InTileMap(spawnPosition);
            if (spawnable) {
                var enemy = Instantiate(
                    enemyPrefab, spawnPosition,
                    Quaternion.identity
                );

                enemy.transform.SetParent(transform, true);
                GameState.AddLivingEnemy(enemy);
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        GameState.StopSpawning();
        yield return null;
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
