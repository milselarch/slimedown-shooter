using UnityEngine;

public class BombSlimeController : SlimeController {
    private static readonly int EXPLODE = Animator.StringToHash("explode");

    public float triggerExplosionDistance = 2.0f;
    // duration between slime detonation and slime explosion
    public float detonationWait = 1.0f;
    public int explosionDamage = 10;
    public AudioSource explosionSound;

    private bool _exploding = false;
    private float _explosionArmingStamp = GameState.DEFAULT_STAMP;

    private bool IsArming() {
        return !GameState.IsApproxEqual(_explosionArmingStamp, GameState.DEFAULT_STAMP);
    }

    private void OnExplodeDone() {
        _exploding = false;
        this.ForceSelfDestruct();
        // Debug.Log("EXPLOSION_END");
    }
    
    public override int GetAttackDamage() {
        return IsExploding() ? explosionDamage : base.GetAttackDamage();
    }

    public bool IsExploding() {
        return _exploding;
    }

    private void FixedUpdate() {
        var updated = DoUpdate();
        if (!updated) { return; }

        var distanceToPlayer = GetPlayerOffset().magnitude;
        var startExplosion = 
            IsArming() && !_exploding &&
            (Time.time - _explosionArmingStamp >= detonationWait);
        var initiateDetonation =
            !IsArming() && !_exploding &&
            (distanceToPlayer <= triggerExplosionDistance);

        if (initiateDetonation) {
            _explosionArmingStamp = Time.time;
            spriteRenderer.material = glowMaterial;
            glowMaterial.SetFloat(shaderStartTime, _explosionArmingStamp);
        } else if (startExplosion) {
            StartExplosion();
        }
    }

    internal override void OnCollisionEnter2D(Collision2D other) {
        if (_exploding) { return; }
        base.OnCollisionEnter2D(other);
    }

    private bool StartExplosion() {
        if (_exploding) { return false; }
        _exploding = true;
        explosionSound.Play();
        // make the explosion unmovable
        this.enemyBody.bodyType = RigidbodyType2D.Static;
        GameState.KillEnemy(baseController.id);

        enemyAnimator.SetBool(EXPLODE, true);
        _explosionArmingStamp = Time.time;
        return true;
    }
}
