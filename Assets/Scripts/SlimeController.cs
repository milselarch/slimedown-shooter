using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System;
using JetBrains.Annotations;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.TextCore;


public class BaseEnemyController {
    public int id { get; private set; } = -1;
    
    public void SetID(int newId) {
        Assert.IsTrue(this.id == -1);
        this.id = newId;
    }
}

public interface IBaseEnemyControllerable {
    public Rigidbody2D enemyBody { get; }
    public bool AttemptSelfDestruct();
    public void Damage(int damage);

    public BaseEnemyController GetBaseController();
    public int GetAttackDamage();
    public int GetEnemyHealth();
}


public class SlimeController: MonoBehaviour, IBaseEnemyControllerable {
	private static readonly int BOUNCE = Animator.StringToHash("bounce");
	private static readonly int ATTACK = Animator.StringToHash("attack");
	private static readonly int DEAD = Animator.StringToHash("dead");
    internal static readonly int shaderStartTime = Shader.PropertyToID("_startTime");
    private static readonly int SHADER_FREQUENCY = Shader.PropertyToID("_frequency");

	public Material defaultMaterial;
    public Material glowMaterialTemplate;
	
    public float jumpForce = 7.0f;
    public float jumpInterval = 1.0f;
    public float attackForce = 11.0f;
    public float attackInterval = 2.0f;
    public float attackDistance = 3.0f;
	// smallest size (relative to original) sprite is allowed to shrink to
	public float smallestScale = 0.75f;
	
    public int maxHealth = 4;
    public int attackDamage = 5;
    
	public IntVariable gameScore;
	// public IntVariable playerHealth;
	public UnityEvent scoreUpdate;
	// public UnityEvent playerHealthUpdate;
	public UnityEvent onEnemyKill;

    public Animator enemyAnimator;

    internal Material glowMaterial;
    internal SpriteRenderer spriteRenderer;
    private BoxCollider2D _boxCollider;
    internal readonly BaseEnemyController baseController = new(); 
    private PlayerController _playerController;
    private NavMeshAgent _agent;
    private GameObject _player;
    
    private float _lastJump = -10.0f;
    private float _lastAttack = -10.0f;
	private bool _dead = false;
	private int _health = 0;

    // Start is called before the first frame update
    private void Start() {
        _boxCollider = GetComponent<BoxCollider2D>();
	    _agent = GetComponent<NavMeshAgent>();
	    _agent.updateRotation = false;
	    _agent.updateUpAxis = false;
	    _agent.isStopped = true;
	    
        enemyBody = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
        _playerController = _player.GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        glowMaterial = new Material(glowMaterialTemplate);
        Respawn();
    }

    public virtual int GetAttackDamage() {
        return attackDamage;
    }
    
    private void Respawn() {
		this._health = this.maxHealth;
        spriteRenderer.material = defaultMaterial;
        enemyAnimator.SetBool(DEAD, false);
        
        _lastJump = GetTimestamp();
        _lastAttack = _lastJump;
		_dead = false;
    }

    public Rigidbody2D enemyBody { get; private set; }

    private static float GetTimestamp() {
        return Time.time;
    }

    /*
    public void SetID(int newId) {
        Assert.IsTrue(this.id == -1);
        this.id = newId;
    }
    */
    
    // inflict damage to the enemy
	public void Damage(int damage = 1) {
		if (AttemptSelfDestruct()) {
			return;
		} 
		
		Assert.IsTrue(_health > 0);
		_health = Math.Max(_health - damage, 0);
		
		gameScore.Increment();
		scoreUpdate.Invoke();

		if (_health == 0) {
			spriteRenderer.material = glowMaterial;
            glowMaterial.SetFloat(shaderStartTime, Time.time);
			enemyAnimator.SetBool(DEAD, true);
			
			transform.localScale = Vector3.one;
            GameState.KillEnemy(baseController.id);
			onEnemyKill.Invoke();
			UpdateCollider();
			_dead = true;
		}
		
		// adjust enemy size based on their current health
		var newScale = smallestScale + (
			(1.0f - smallestScale) * ((float) _health) / maxHealth
		);
		transform.localScale = Vector3.one * newScale;
		UpdateCollider();
	}

	private void UpdateCollider() {
		// adjust bounding box to fit new sprite scale accordingly
        var sprite = spriteRenderer.sprite;
        _boxCollider.size = sprite.bounds.size;
        _boxCollider.offset = sprite.bounds.center;
	}
    
    private void OnWillRenderObject() {
        UpdateCollider();
    }

    internal virtual void OnCollisionEnter2D(Collision2D other) {
        var player = other.gameObject.GetComponent<PlayerController>();
        if (player != null) {
            AttemptSelfDestruct();
            return;
        }

        var bombSlime = other.gameObject.GetComponent<BombSlimeController>();
        if (bombSlime != null) {
            if (bombSlime.IsExploding()) {
                this.Damage(bombSlime.explosionDamage);
            }
        }
        
        var bullet  = other.gameObject.GetComponent<BlasterShotController>();
        if (bullet != null) {
            this.Damage(BlasterShotController.GetAttackDamage());
        }
    }

    public void GameRestart() {
        // destroy self and deregister from GameState
        _health = 0;
        Destroy(gameObject);
        GameState.KillEnemy(baseController.id);
        // Debug.Log("GameState.GameRestart");
    }

	public bool AttemptSelfDestruct() {
        if (_health != 0) {
            return false;
        }

        Destroy(gameObject);
        GameState.KillEnemy(baseController.id);
        onEnemyKill.Invoke();
        return true;
    }

    protected void ForceSelfDestruct() {
        _health = 0;
        this.AttemptSelfDestruct();
    }

	public int GetEnemyHealth() {
		return this._health;
	}

	public bool IsAlive() {
		return this._health > 0;
	}

    private void FixedUpdate() {
        DoUpdate();
    }

    private Vector3 GetMovementVector(Vector3 playerVectorDist) {
        var playerPosition = _playerController.GetPosition2D();
        
        var path = new NavMeshPath();
        var hasNavMeshPath = _agent.CalculatePath(playerPosition, path);
        var movementDirection = playerVectorDist;
			
        if (hasNavMeshPath && (path.corners.Length > 1)) {
            // use NavMeshPlus AI to calculate movement direction
            movementDirection = path.corners[1] - transform.position;
        }

        return movementDirection;
    }

    internal Vector3 GetPlayerOffset() {
        return _player.transform.position - transform.position;
    }

    internal bool DoUpdate() {
		if (_dead) { return false; }
        
        var playerVectorDist = GetPlayerOffset();
		var movementDirection = GetMovementVector(playerVectorDist);
		
		var timestampNow = GetTimestamp();
        var jumpDurationPassed = timestampNow - _lastJump;
        var attackDurationPassed = timestampNow - _lastAttack;
        var alignment = Vector3.Dot(
	        playerVectorDist.normalized, movementDirection.normalized
	    );

	    var hasLineOfSight = alignment >= 0.99;
	    var playerDirection = movementDirection.normalized;
        var playerDistance = movementDirection.magnitude;
        // Debug.Log("PLAYER_DISTANCE= " + playerDistance);

        if (hasLineOfSight && (playerDistance < this.attackDistance)) {
            if (!(attackDurationPassed > attackInterval)) {
                return true;
            }

            _lastAttack = timestampNow;
            enemyBody.AddForce(playerDirection * attackForce, ForceMode2D.Impulse);
            enemyAnimator.SetTrigger(ATTACK);
        } else if (jumpDurationPassed > jumpInterval) {
			// jump towards the player
            _lastJump = timestampNow;
            enemyBody.AddForce(playerDirection * jumpForce, ForceMode2D.Impulse);
            enemyAnimator.SetTrigger(BOUNCE);
        }

        return true;
    }

    public BaseEnemyController GetBaseController() {
        return this.baseController;
    }
}
