using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using System;
using JetBrains.Annotations;

public class EnemyController: MonoBehaviour {
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

    private NavMeshAgent Agent;

    private Rigidbody2D enemyBody;
    private GameObject player;
    private float lastJump = -10.0f;
    private float lastAttack = -10.0f;
	private bool dead = false;
	private int health = 0;

    // Start is called before the first frame update
    void Start() {
	    Agent = GetComponent<NavMeshAgent>();
	    Agent.updateRotation = false;
	    Agent.updateUpAxis = false;
	    Agent.isStopped = true;
	    
        enemyBody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
		GameRestart();
    }

    public void GameRestart() {
		this.health = this.maxHealth;
        enemyAnimator.SetBool("dead", false);
        lastJump = GetTimestamp();
        lastAttack = lastJump;
		dead = false;
    }

    float GetTimestamp() {
        return Time.time;
    }

    public void Damage() {
	    this.Damage(1);
    }
    
	// inflict damage to the enemy
	public void Damage(int damage) {
		if (health == 0) {
			Destroy(gameObject);
			return;
		}

		if (health > 0) {
			health = Math.Max(health - damage, 0);
		} 
		
		gameScore.Increment();
		scoreUpdate.Invoke();

		if (health == 0) {
			enemyAnimator.SetBool("dead", true);
			transform.localScale = Vector3.one;
			
			onEnemyKill.Invoke();
			UpdateCollider();
			dead = true;
		}
		
		// adjust enemy size based on their current health
		float newScale = smallestScale + (
			(1.0f - smallestScale) * ((float) health) / maxHealth
		);
		transform.localScale = Vector3.one * newScale;
		UpdateCollider();
	}

	void UpdateCollider() {
		BoxCollider2D col = gameObject.GetComponent<BoxCollider2D>();
        if (col) {
			// adjust bounding box to fit new sprite scale accordingly
			col.size = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
			col.offset = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.center;
		}
	}

	private void OnCollisionEnter2D(Collision2D other) {
		if (
			(other.gameObject.CompareTag("Player"))
		) {
			if (health == 0)
			{
				Destroy(gameObject);
			}
		}
	}

	public void AttemptSelfDestruct() {
		if (health == 0) {
			Destroy(gameObject);
		}
	}

	public int GetEnemyHealth() {
		return this.health;
	}

	public bool IsAlive() {
		return this.health > 0;
	}

    void FixedUpdate() {
		if (dead) { return; }

		var playerController = player.GetComponent<PlayerController>();
		Vector2 playerPosition = playerController.getPosition2D();

		Vector3 playerVectorDist;
		NavMeshPath path = new NavMeshPath();
		var hasNavMeshPath = Agent.CalculatePath(playerPosition, path);
		
		if (hasNavMeshPath && (path.corners.Length > 1)) {
			// use NavMeshPlus AI to calculate movement direction
			playerVectorDist = path.corners[1] - transform.position;
		} else {
			// use vector from enemy to player to calculate movement direction
			playerVectorDist = player.transform.position - transform.position;
		}
		
        float timestampNow = GetTimestamp();
        float jumpDurationPassed = timestampNow - lastJump;
        float attackDurationPassed = timestampNow - lastAttack;

        Vector3 playerDirection = playerVectorDist.normalized;
        float playerDistance = playerVectorDist.magnitude;
        // Debug.Log("PLAYER_DISTANCE= " + playerDistance);

        if (playerDistance < this.attackDistance) {
            if (attackDurationPassed > attackInterval) {
				// jump-attack towards the player
				// TODO: only use jump-attack if the player is
				// in the line of sight of the enemy slime
                lastAttack = timestampNow;
                enemyBody.AddForce(playerDirection * attackForce, ForceMode2D.Impulse);
                enemyAnimator.SetTrigger("attack");
            }
        } else if (jumpDurationPassed > jumpInterval) {
			// jump towards the player
            lastJump = timestampNow;
            enemyBody.AddForce(playerDirection * jumpForce, ForceMode2D.Impulse);
            enemyAnimator.SetTrigger("bounce");
        }
    }

    // Update is called once per frame
    void Update() {
	    
    }
}
