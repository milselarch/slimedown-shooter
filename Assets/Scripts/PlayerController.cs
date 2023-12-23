using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour {
    public float maxSpeed = 20;
    public float impulseForce = 5.0f;
    public float speed = 10;
    public float fireBallOffset = 1.0f;
    public float chargeWaitDuration = 1.0f;
    public GameObject attackPrefab;
    public float chargeForce = 5.0f;
    public Animator playerAnimator;
    
    // track and manage player's health and game score
    public IntVariable health;
    public IntVariable gameScore;

    public UnityEvent scoreUpdate;
    public UnityEvent playerHealthUpdate;

	// whether or not player is allowed to fire projectile
	// private bool canFire = false;
    private int _horizontalDirection = 0;
    private int _verticalDirection = 0;
    private bool _faceRight = true;
    
    // keep track of which side the mario sprite is facing
    private SpriteRenderer _playerSprite;
    private Rigidbody2D _playerBody;
    private float _lastCharge = 0.0f;
    private bool _charging = false;
    private bool _dead = false;

    // Start is called before the first frame update
    void Start() {
        // Set to be 30 FPS
        Application.targetFrameRate =  30;
        
        // assign mario sprite object
        _playerSprite = GetComponent<SpriteRenderer>();
        _playerBody = GetComponent<Rigidbody2D>();
        GameRestart();
    }

    public void OnHealthUpdate() {
        if (health.Value <= 0) {
            _dead = true;
        }   
    }

    public Vector2 getPosition2D() {
        Vector3 position = this.transform.position;
        return new Vector2(position.x, position.y);
    }

    void GameRestart() {
        // reset sprite direction
        _faceRight = true;
        _playerSprite.flipX = false;
        _charging = false;
        _dead = false;
    }

    void FixedUpdate() {
        if (_dead) { return; }
        
        // this.canFire = true;
        float xMovement = 0.0f;
        float yMovement = 0.0f;
        
        if (Math.Abs(_playerBody.velocity.x) < maxSpeed) {
            xMovement = this.speed * this._horizontalDirection;
        } 
        if (Math.Abs(_playerBody.velocity.y) < maxSpeed) {
            yMovement = this.speed * this._verticalDirection;
        }
        
        Vector2 movement = new Vector2(xMovement, yMovement);
        _playerBody.AddForce(movement);
    }
    
    public void OnMouseClick(InputAction.CallbackContext context) {
        if (_dead) { return; }

        // Check if the mouse button was pressed
        if (!context.started) { return; }
        // if (!this.canFire) {}
    
        // Get the position of the mouse click relative to the center of the screen
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 mouseOffset = mousePosition - center;

        // Log the mouse click position relative to the center
        Debug.Log("Mouse click offset from center: " + mouseOffset);
        Vector2 direction = mouseOffset.normalized;
        Vector3 direction3 = new Vector3(
            direction.x, direction.y, 0.0f
        );

        GameObject x = Instantiate(
            attackPrefab, 
            transform.position + direction3 * fireBallOffset, 
            Quaternion.identity
        );
        // Get the Rigidbody component of the instantiated object
        Rigidbody2D rb = x.GetComponent<Rigidbody2D>();
        // Check if the Rigidbody component exists
        if (rb != null) {
            // Apply a rightward impulse force to wsthe object
            rb.AddForce(direction * impulseForce, ForceMode2D.Impulse);
            health.Decrement(2, 0);
            playerHealthUpdate.Invoke();
        }
    }

    public void OnChargeAttack(InputAction.CallbackContext context) {
        if (!context.started) { return; }
        if (_charging) { return; }
        if (_dead) { return; }

        float timestamp = Time.time;
        if (timestamp - _lastCharge > chargeWaitDuration) {
            _lastCharge = chargeWaitDuration;
            int faceDirection = this._faceRight ? 1 : -1;
            
            Vector2 chargeDirection;
            if (_playerBody.velocity.magnitude > 0) {
                chargeDirection = _playerBody.velocity.normalized;
            } else {
                chargeDirection = new Vector2(faceDirection, 0.0f);
            }

            chargeDirection.x = Math.Abs(chargeDirection.x) * faceDirection;
            _playerBody.AddForce(chargeForce * chargeDirection, ForceMode2D.Impulse);
            playerAnimator.SetTrigger("charging");
            health.Decrement(1, 0);
            playerHealthUpdate.Invoke();
            _charging = true;
        }
    }

    public void EndCharge() {
        _charging = false;
    }

    public void OnHorizontalMoveAction(InputAction.CallbackContext context) {
        if (context.started) {
            int faceRight = context.ReadValue<float>() > 0 ? 1 : -1;
            if (faceRight != 0) {
                this._faceRight = faceRight == 1;
                _playerSprite.flipX = !this._faceRight;
            }
            _horizontalDirection = faceRight;
        }
        if (context.canceled) {
            _horizontalDirection = 0;
        }
    }
    
    public void OnVerticalMoveAction(InputAction.CallbackContext context)
    {
        if (context.started) {
            int faceTop = context.ReadValue<float>() > 0 ? 1 : -1;
            _verticalDirection = faceTop;
        }
        if (context.canceled) {
            _verticalDirection = 0;
        }
    }

    // Update is called once per frame
    void Update() {
        float velocity = Mathf.Abs(_playerBody.velocity.magnitude);
        playerAnimator.SetFloat("speed", velocity);
    }

    public void DrainHealth(int amount) {
        // drain player health by 1
        health.Decrement(amount, 0);
        playerHealthUpdate.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        if (other.gameObject.CompareTag("Enemy")) {
            int enemyHealth = other.gameObject
                .GetComponent<EnemyController>().GetEnemyHealth();
            
            if (_charging) {
                // charging the enemy increases player health by 1
                other.gameObject.GetComponent<EnemyController>().Damage(2);
                gameScore.Increment();
                health.Increment(1, 100);
                
                int newEnemyHealth = other.gameObject
                    .GetComponent<EnemyController>().GetEnemyHealth();

                if (newEnemyHealth == 0) {
                    // if charging killed the slime automatically
                    // destroy its sprite also
                    other.gameObject
                        .GetComponent<EnemyController>().AttemptSelfDestruct();
                    health.Increment(14, 100);
                }
                
                playerHealthUpdate.Invoke();

            } else if (enemyHealth == 0) {
                // increase health after collecting dead slime
                health.Increment(14, 100);
                gameScore.Increment();
                playerHealthUpdate.Invoke();
                scoreUpdate.Invoke();
            } else {
                // slime deals 5 damage to player
                health.Decrement(5, 0);
                playerHealthUpdate.Invoke();
            }
        }
    }
}
