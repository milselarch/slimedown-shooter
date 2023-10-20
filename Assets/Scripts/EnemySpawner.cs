using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

public class EnemySpawner : MonoBehaviour
{
    // Start is called before the first frame update
    public Tilemap tilemap;
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
    private bool _spawning = false;
    
    private int _lastSpawnedWave = 0;
    private int _enemiesKilled = 0;
    private int _spawned = 0;

    void Start()
    {
        waveCounter.SetValue(0);
        waveTimestamp.SetValue(0);
        _startPosition = transform.position;
        StartCoroutine(SpawnAttackWave());
        StartCoroutine(TimerUpdateLoop());
    }
    
    IEnumerator TimerUpdateLoop()
    {
        /*
         * Drains player health every 3 seconds if he's
         * ran out of time to complete the current attack wave
         */
        while (!_destroyed)
        {
            int timestampNow = (int) Time.time;
            int allowedWaveDuration = 20 + 5 * waveCounter.Value;
            int durationPassed = timestampNow - waveTimestamp.Value;
            int durationLeft = allowedWaveDuration - durationPassed;

            if (durationLeft <= -1) {
                int penalty = (int) Math.Log10(-durationLeft) + 1;
                timerDrainHealth.Invoke(penalty);
            }
            
            yield return new WaitForSeconds(3.0f);
        }

        yield return null;
    }

    public void OnEnemyKilled() {
        _enemiesKilled += 1;

        // check if any enemies from the previous wave are still alive
        bool hasLivingEnemies = false;
        foreach (Transform child in transform)
        {
            bool alive = child.GetComponent<EnemyController>().IsAlive();
            if (alive)
            {
                hasLivingEnemies = true;
                break;
            }
        }
        
        if (!hasLivingEnemies && !_spawning) {
            Debug.Log("WAVE IS OVER");
            StartCoroutine(SpawnAttackWave());
            waveUpdate.Invoke();
        }
    }

    IEnumerator SpawnAttackWave()
    {
        /*
         * spawn slimes in an attack wave
         */
        while (_spawning) {
            // wait for previous wave to finish spawning
            yield return null;
        }
        
        _spawning = true;
        _enemiesKilled = 0;
        waveCounter.Increment();
        _spawned = 0;

        // set start of attack wave to right now
        waveTimestamp.SetValue((int) Time.time);
        
        while (_spawned < waveCounter.Value + 3)
        {
            float offsetX = 2 * Random.Range(0.0f, spawnRadius) - spawnRadius;
            float offsetY = 2 * Random.Range(0.0f, spawnRadius) - spawnRadius;
            Vector3 spawnPosition = new Vector3(
                _startPosition.x + offsetX,
                _startPosition.y + offsetY,
                _startPosition.z
            );

            bool spawnable = InTilemap(spawnPosition);
            if (spawnable) {
                GameObject enemy = Instantiate(
                    enemyPrefab, spawnPosition,
                    Quaternion.identity
                );

                enemy.transform.SetParent(transform, true);
                _spawned += 1;
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        _spawning = false;
        yield return null;
    }
    
    private bool InTilemap(Vector3 position) {
        // check if the spawn position is in spawnable tile area
        Vector3Int cellPosition = tilemap.WorldToCell(position);
        return tilemap.HasTile(cellPosition);
    }

    private void OnDestroy()
    {
        _destroyed = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
