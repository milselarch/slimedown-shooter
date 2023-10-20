using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EnemyController : MonoBehaviour
{
    public float jumpForce = 7.0f;
    public float jumpInterval = 1.0f;
    
    private Rigidbody2D enemyBody;
    private GameObject player;
    private float lastJump = 0.0f;
    
    // Start is called before the first frame update
    void Start()
    {
        enemyBody = GetComponent<Rigidbody2D>();
        player = GameObject.FindGameObjectWithTag("Player");
        lastJump = getTimestamp();
    }

    float getTimestamp()
    {
        return Time.time;
    }

    void FixedUpdate()
    {
        float timestampNow = getTimestamp();
        float durationPassed = timestampNow - lastJump;
        Debug.Log("durationPassed " + durationPassed);
        
        if (durationPassed > jumpInterval)
        {
            lastJump = timestampNow;
            Vector3 playerDirection = (player.transform.position - transform.position).normalized;
            enemyBody.AddForce(playerDirection * jumpForce, ForceMode2D.Impulse);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
