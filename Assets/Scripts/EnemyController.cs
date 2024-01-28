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

public class EnemyController: MonoBehaviour {
	private static readonly int BOUNCE = Animator.StringToHash("bounce");
	private static readonly int ATTACK = Animator.StringToHash("attack");
	private static readonly int DEAD = Animator.StringToHash("dead");

    private int id { get; set; } = -1;
	public Material defaultMaterial;
	public Material glowMaterial;
	
    public float jumpForce = 7.0f;
    public float jumpInterval = 1.0f;
    public float attackForce = 11.0f;
    public float attackInterval = 2.0f;
    public float attackDistance = 3.0f;
	// smallest size (relative to original) sprite is allowed to shrink to
	public float smallestScale = 0.75f;
	public int maxHealth = 4;
    
	public IntVariable gameScore;
	// public IntVariable playerHealth;
	public UnityEvent scoreUpdate;
	// public UnityEvent playerHealthUpdate;
	public UnityEvent onEnemyKill;

    public Animator enemyAnimator;

    private NavMeshAgent _agent;
    private Rigidbody2D _enemyBody;
    private GameObject _player;
    private float _lastJump = -10.0f;
    private float _lastAttack = -10.0f;
	private bool _dead = false;
	private int _health = 0;

    // Start is called before the first frame update
    void Start() {
	    _agent = GetComponent<NavMeshAgent>();
	    _agent.updateRotation = false;
	    _agent.updateUpAxis = false;
	    _agent.isStopped = true;
	    
        _enemyBody = GetComponent<Rigidbody2D>();
        _player = GameObject.FindGameObjectWithTag("Player");
		GameRestart();
    }

    private void GameRestart() {
		this._health = this.maxHealth;
		GetComponent<SpriteRenderer>().material = defaultMaterial;
        enemyAnimator.SetBool(DEAD, false);
        
        _lastJump = GetTimestamp();
        _lastAttack = _lastJump;
		_dead = false;
    }

    public Rigidbody2D enemyBody => _enemyBody;

    private static float GetTimestamp() {
        return Time.time;
    }

    public void SetID(int newId) {
        Assert.IsTrue(this.id == -1);
        this.id = newId;
    }
    
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
			GetComponent<SpriteRenderer>().material = glowMaterial;
			enemyAnimator.SetBool(DEAD, true);
			
			transform.localScale = Vector3.one;
            GameState.KillEnemy(id);
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
		var col = gameObject.GetComponent<BoxCollider2D>();
		if (!col) { return; }

		// adjust bounding box to fit new sprite scale accordingly
		col.size = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
		col.offset = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.center;
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if ((!other.gameObject.CompareTag("Player"))) {
			return;
		}

        AttemptSelfDestruct();
    }

	public bool AttemptSelfDestruct() {
        if (_health != 0) {
            return false;
        }

        Destroy(gameObject);
        GameState.KillEnemy(id);
        return true;
    }

	public int GetEnemyHealth() {
		return this._health;
	}

	public bool IsAlive() {
		return this._health > 0;
	}

    private void FixedUpdate() {
		if (_dead) { return; }

		var playerController = _player.GetComponent<PlayerController>();
		var playerPosition = playerController.GetPosition2D();

		var path = new NavMeshPath();
		var hasNavMeshPath = _agent.CalculatePath(playerPosition, path);
		var playerVectorDist = _player.transform.position - transform.position;
		var movementDirection = playerVectorDist;
			
		if (hasNavMeshPath && (path.corners.Length > 1)) {
			// use NavMeshPlus AI to calculate movement direction
			movementDirection = path.corners[1] - transform.position;
		}
		
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
	        if (!(attackDurationPassed > attackInterval)) { return; }
	        
	        _lastAttack = timestampNow;
	        _enemyBody.AddForce(playerDirection * attackForce, ForceMode2D.Impulse);
	        enemyAnimator.SetTrigger(ATTACK);
        } else if (jumpDurationPassed > jumpInterval) {
			// jump towards the player
            _lastJump = timestampNow;
            _enemyBody.AddForce(playerDirection * jumpForce, ForceMode2D.Impulse);
            enemyAnimator.SetTrigger(BOUNCE);
        }
    }

    // Update is called once per frame
    void Update() {
	    
    }
}
