using UnityEngine;

public class BombSlimeController: SlimeController {
    private static readonly int EXPLODE = Animator.StringToHash("explode");

    public float triggerExplosionDistance = 10.0f; 
    
    private bool _exploding = false;
    private float fuseExplosionStamp = -1.0f; 
    
    private void FixedUpdate() {
        var updated = DoUpdate();
        if (!updated) { return; }

        var distanceToPlayer = GetPlayerOffset().magnitude;
        if (distanceToPlayer < triggerExplosionDistance) {
            // enemyAnimator.SetBool(EXPLODE, true);
            _exploding = true;
        }
    }
}
