using UnityEngine;

public class GameState: MonoBehaviour {
    private static bool _paused = false;
    public static IntVariable health;

    public static bool dead {
        get {
            if (!health) { return false; }
            return health.Value <= 0;
        }
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

