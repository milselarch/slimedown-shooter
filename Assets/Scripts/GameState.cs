using System;
using System.Collections.Generic;
using ScriptableObjects;
using UnityEngine;
using UnityEngine.Assertions;

public class GameState: MonoBehaviour {
    private static readonly Dictionary<int, BaseEnemyController> LIVING_ENEMIES = new();
    private const float TOLERANCE = 0.001f;
    // ReSharper disable once InconsistentNaming
    public const float DEFAULT_STAMP = -1.0f;
    private static bool _paused = false;
    
    public static IntVariable health;
    public static int spawned { get; private set; } = 0;
    public static bool spawning { get; private set; } = false;

    public static void ResetWaveSpawnFlags() {
        spawned = 0;
        Assert.IsFalse(spawning);
        spawning = true;
    }

    public static void KillEnemy(int id) {
        // Debug.Log("COUNT_ENEMY " + LIVING_ENEMIES.Count);
        // Debug.Log("KILL_ENEMY: " + id);
        LIVING_ENEMIES.Remove(id);
    }

    public static void StopSpawning() {
        spawning = false;
    }

    private static int GetNumLivingEnemies() {
        return LIVING_ENEMIES.Count;
    }

    public static bool HasLivingEnemies() {
        return GetNumLivingEnemies() > 0;
    }

    public static void AddLivingEnemy(BaseEnemyController enemy) {
        enemy.SetID(spawned);
        LIVING_ENEMIES[spawned] = enemy;
        spawned += 1;
    }

    public static bool dead {
        get {
            if (!health) { return false; }
            return health.value <= 0;
        }
    }

    public static bool IsApproxEqual(float num1, float num2) {
        return Math.Abs(num1 - num2) < TOLERANCE;
    }

    public static bool allowPlayerAction => !(
        GameState.dead || GameState.paused
    );
    
    public static bool paused {
        get => _paused;
        set {
            _paused = value;
            Time.timeScale = _paused ? 0.0f : 1.0f;
        }
    }
}

