using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class EnemyController: MonoBehaviour
{
    public float jumpForce = 7.0f;
    public float jumpInterval = 1.0f;

    public float attackForce = 11.0f;
    public float attackInterval = 2.0f;
    public float attackDistance = 3.0f;
	// smallest size (relative to original) sprite is allowed to shrink to
	public float smallestScale = 0.75f;
	public int maxHealth = 4;
    
	public IntVariable playerHealth;
	public IntVariable gameScore;
	public UnityEvent scoreUpdate;
	public UnityEvent playerHealthUpdate;

    public Animator enemyAnimator;

    private Rigidbody2D enemyBody;
    private GameObject player;
    private float lastJump = 0.0f;
    private float lastAttack = 0.0f;
	private bool dead = false;
	private int health = 0;

    // Start is called before the first frame update
    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
		GameRestart();
    }

    public void GameRestart()
    {
		this.health = this.maxHealth;
        enemyAnimator.SetBool("dead", false);
        lastJump = GetTimestamp();
        lastAttack = lastJump;
		dead = false;
    }

    float GetTimestamp()
    {
        return Time.time;
    }

	// inflict damage to the enemy
	public void damage() {
		if (health == 0) { return; }
		if (health > 0) { health -= 1; } 
		
		gameScore.Increment();
		scoreUpdate.Invoke();

		if (health == 0) {
			enemyAnimator.SetBool("dead", true);
			transform.localScale = Vector3.one;
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
		BoxCollider2D collider = gameObject.GetComponent<BoxCollider2D>();
        if (collider) {
			// adjust bounding box to fit new sprite scale accordingly
			collider.size = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.size;
       		collider.offset = gameObject.GetComponent<SpriteRenderer>().sprite.bounds.center;
		}
	}

	private void OnCollisionEnter2D(Collision2D other)
	{
		if (other.gameObject.CompareTag("Player")) {
			if (health == 0)
			{
				Destroy(gameObject);
				playerHealth.ApplyChange(1, 100);
				gameScore.Increment();
				playerHealthUpdate.Invoke();
				scoreUpdate.Invoke();
			}
		}
	}

    void FixedUpdate()
    {
		if (dead) { return; }

        float timestampNow = GetTimestamp();
        float jumpDurationPassed = timestampNow - lastJump;
        float attackDurationPassed = timestampNow - lastAttack;

        Vector3 playerVectorDist = player.transform.position - transform.position;
        Vector3 playerDirection = playerVectorDist.normalized;
        float playerDistance = playerVectorDist.magnitude;
        // Debug.Log("PLAYER_DISTANCE= " + playerDistance);

        if (playerDistance < this.attackDistance) {
            if (attackDurationPassed > attackInterval) {
				// jump-attack towards the player
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
    void Update()
    {
        
    }
}
