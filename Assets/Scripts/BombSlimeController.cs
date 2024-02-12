using UnityEngine;

public class BombSlimeController : SlimeController {
    private static readonly int EXPLODE = Animator.StringToHash("explode");

    public float triggerExplosionDistance = 10.0f;

    private bool _exploding = false;
    private float _detonationStamp = GameState.DEFAULT_STAMP;

    private bool IsDetonating() {
        return !GameState.IsApproxEqual(_detonationStamp, GameState.DEFAULT_STAMP);
    }

    private void FixedUpdate() {
        var updated = DoUpdate();
        if (!updated) { return; }

        var distanceToPlayer = GetPlayerOffset().magnitude;
        if (!IsDetonating() && (distanceToPlayer < triggerExplosionDistance)) {
            // enemyAnimator.SetBool(EXPLODE, true);
            _detonationStamp = Time.time;
            spriteRenderer.material = glowMaterial;
            glowMaterial.SetFloat(SHADER_START_TIME, _detonationStamp);
        }
    }
}