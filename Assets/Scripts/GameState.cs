using System;
using UnityEngine;

public class GameState: MonoBehaviour {
    public const float TOLERANCE = 0.001f;
    private static bool _paused = false;
    public static IntVariable Health;

    public static bool Dead {
        get {
            if (!Health) { return false; }
            return Health.Value <= 0;
        }
    }

    public static bool IsApproxEqual(float num1, float num2) {
        return Math.Abs(num1 - num2) < TOLERANCE;
    }

    public static bool AllowPlayerAction => !(
        GameState.Dead || GameState.Paused
    );
    
    public static bool Paused {
        get => _paused;
        set {
            _paused = value;
            Time.timeScale = _paused ? 0.0f : 1.0f;
        }
    }
}

