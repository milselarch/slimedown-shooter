using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    public float jumpForce = 7.0f;
    public float jumpInterval = 1.0f;

    public float attackForce = 11.0f;
    public float attackInterval = 2.0f;
    public float attackDistance = 3.0f;
    
    public Animator enemyAnimator;

    private Rigidbody2D enemyBody;
    private GameObject player;
    private float lastJump = 0.0f;
    private float lastAttack = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        lastJump = getTimestamp();
        lastAttack = lastJump;
    }

    public void GameRestart()
    {
        enemyAnimator.SetBool("dead", false);
    }

    float getTimestamp()
    {
        return Time.time;
    }

    void FixedUpdate()
    {
        float timestampNow = getTimestamp();
        float jumpDurationPassed = timestampNow - lastJump;
        float attackDurationPassed = timestampNow - lastAttack;

        Vector3 playerVectorDist = player.transform.position - transform.position;
        Vector3 playerDirection = playerVectorDist.normalized;
        float playerDistance = playerVectorDist.magnitude;
        Debug.Log("PLAYER_DISTANCE= " + playerDistance);

        if (playerDistance < this.attackDistance) {
            if (attackDurationPassed > attackInterval) {
                lastAttack = timestampNow;
                enemyBody.AddForce(playerDirection * attackForce, ForceMode2D.Impulse);
                enemyAnimator.SetTrigger("attack");
            }
        } else if (jumpDurationPassed > jumpInterval) {
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
