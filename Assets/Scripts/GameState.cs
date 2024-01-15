using UnityEngine;

public class GameState {
    private static bool _paused = false;

    public static bool paused {
        get => _paused;
        set {
            _paused = value;
            Time.timeScale = _paused ? 0.0f : 1.0f;
        }
    }
}

