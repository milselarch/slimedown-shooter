using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class PlayerController : MonoBehaviour {
    private const string FALLING_SORTING_LAYER = "falling";
    private const float FALL_DAMAGE_WAIT = 0.005f;
    
    private static readonly int CHARGING = Animator.StringToHash("charging");
    private static readonly int SPEED = Animator.StringToHash("speed");
    private static readonly int BOUNDARY_Y = Shader.PropertyToID("_boundaryY");
    
    // whether player input controls are disabled
    public bool disableInputs = false;
    
    public float maxSpeed = 20;
    public float impulseForce = 5.0f;
    public float speed = 10;
    public float fireBallOffset = 1.0f;
    public float chargeWaitDuration = 1.0f;
    public float chargeForce = 5.0f;
    public float damageTintDuration = 0.3f;
    
    public Tilemap tileMap;
    public GameObject attackPrefab;
    public Animator playerAnimator;
    
    // track and manage player's health and game score
    public IntVariable health;
    public IntVariable gameScore;
    public Color damageTint;

    public UnityEvent scoreUpdate;
    public UnityEvent playerHealthUpdate;
    public GameConstants gameConstants;
    public InputActionAsset actionAsset;

	// whether player is allowed to fire projectile
	// private bool canFire = false;
    private int _horizontalDirection = 0;
    private int _verticalDirection = 0;
    private bool _faceRight = true;

    // keep track of which side the mario sprite is facing
    private SpriteRenderer _playerSprite;
    private Rigidbody2D _playerBody;
    private Vector3 _startPosition;
    private Color _originalColor;
    
    private float _lastFallTime = GameState.DEFAULT_STAMP;
    private float _lastFallDamageTime = GameState.DEFAULT_STAMP;
    private float _lastDamagedTime = GameState.DEFAULT_STAMP;
    private float _lastCharge = 0.0f;
    
    private bool _charging = false;
    private bool _destroyed = false;
    private int _defaultSortingLayerID;
    private int _fallingSortingLayerID;
    private int _maxHealth;
    
    private Vector3 _bottomLeft;
    private Vector3 _bottomRight;

    // Start is called before the first frame update
    private void Start() {
        if (!Application.isEditor) {
            Application.targetFrameRate = 60;
        }
        
        _fallingSortingLayerID = SortingLayer.NameToID(FALLING_SORTING_LAYER);
        _maxHealth = gameConstants.startMaxHealth;
        _startPosition = transform.position;
            
        GameState.health = health;
        _lastCharge = -chargeWaitDuration;
        _playerSprite = GetComponent<SpriteRenderer>();
        _defaultSortingLayerID = _playerSprite.sortingLayerID;
        
        StartCoroutine(FadeDamageEffect());
        health.AttachCallback(OnHealthChange);
        UpdateHealthBar(health.Value);
        
        _playerBody = GetComponent<Rigidbody2D>();
        _originalColor = _playerSprite.color;
        GameRestart();
    }

    private void OnDrawGizmos() {
        MarkTileMapPositions();
    }

    private void ResetInputDevices() {
        actionAsset.Disable();
        actionAsset.Enable();
    }

    private void MarkTileMapPositions() {
        /*
         * Draw a wireframe cube around each tile in the tilemap
         * in the unity editor
         */
        #if UNITY_EDITOR
        if (tileMap == null) { return; }
        
        Gizmos.color = Color.red;
        var bounds = tileMap.cellBounds;
        var allTiles = tileMap.GetTilesBlock(bounds);
        const float length = 0.9f;

        for (var x = 0; x < bounds.size.x; x++) {
            for (var y = 0; y < bounds.size.y; y++) {
                var tile = allTiles[x + y * bounds.size.x];
                if (tile == null) {
                    continue;
                }

                var localPlace = new Vector3Int(
                    x, y, (int) tileMap.transform.position.z
                ) + bounds.position;
                var leftCornerPos = tileMap.CellToWorld(localPlace);
                var centerPos = new Vector3(
                    leftCornerPos.x + length / 2, leftCornerPos.y + length / 2,
                    leftCornerPos.z
                ); 

                Gizmos.DrawWireCube(centerPos, new Vector3(
                    length, length, length
                ));
                Handles.Label(centerPos, $"({localPlace.x}, {localPlace.y})");
            }
        }
        
        Gizmos.color = Color.green;
        Gizmos.DrawLine(_bottomLeft, _bottomRight);
        #endif
    }

    private void SetColliderEnabled(bool enable) {
        var boxCollider2D = GetComponent<BoxCollider2D>();
        if (boxCollider2D != null) {
            boxCollider2D.enabled = enable;
        }
    }

    public float GetChargeProgress() {
        // check what fraction of required wait time for charge attack is completed 
        var timePassed = Time.time - _lastCharge;
        var progress = Math.Min(timePassed / chargeWaitDuration, 1.0f);
        return progress;
    }

    private bool GetChargeWaitDone() {
        return GetChargeProgress() >= 1.0f;
    }

    private bool IsFalling() {
        return !GameState.IsApproxEqual(_lastFallTime, GameState.DEFAULT_STAMP);
    }

    private void OnHealthChange(int prevHealth, int newHealth) {
        UpdateHealthBar(newHealth);
    }

    private void UpdateHealthBar(int currentHealth) {
        if (_playerSprite == null) { return; }
        var healthRatio = currentHealth / (float) _maxHealth;
        var threshold = healthRatio - 0.5f;
        var propertyBlock = new MaterialPropertyBlock();
        propertyBlock.SetFloat(BOUNDARY_Y, threshold);
        _playerSprite.SetPropertyBlock(propertyBlock);
    }

    private IEnumerator FadeDamageEffect() {
        yield return null;

        while (!_destroyed) {
            yield return null;
            if (GameState.IsApproxEqual(_lastDamagedTime, GameState.DEFAULT_STAMP)) {
                continue;                
            }
            var timePassed = Time.time - _lastDamagedTime;
            if (timePassed > damageTintDuration) {
                continue;
            }
            
            _playerSprite.color = Color.Lerp(
                damageTint, _originalColor, timePassed / damageTintDuration
            );
        }
    }

    private void OnDestroy() {
        _destroyed = true;
        health.ClearCallbacks();
    }

    public Vector2 GetPosition2D() {
        var position = this.transform.position;
        return new Vector2(position.x, position.y);
    }

    public void GameRestart() {
        // zero out movement
        _horizontalDirection = 0;
        _verticalDirection = 0;
        
        transform.position = _startPosition;
        _playerBody.velocity = Vector2.zero;
        _playerBody.gravityScale = 0.0f;
        
        // reset sprite direction
        _faceRight = true;
        _playerSprite.flipX = false;
        _charging = false;
        _playerSprite.sortingLayerID = _defaultSortingLayerID;

        // reset fall damage counters
        _lastFallTime = GameState.DEFAULT_STAMP;
        _lastFallDamageTime = GameState.DEFAULT_STAMP;
        
        GameState.paused = false;
        gameScore.SetValue(0);
        health.SetValue(gameConstants.startMaxHealth);
        playerHealthUpdate.Invoke();
        scoreUpdate.Invoke();
        
        SetColliderEnabled(true);
        Input.ResetInputAxes();
        ResetInputDevices();
    }

    private void FixedUpdate() {
        if (IsFalling()) {
            var fallDurationPassed = Time.time - _lastFallDamageTime;
            if (fallDurationPassed > FALL_DAMAGE_WAIT) {
                _lastFallDamageTime = Time.time;
                DrainHealth(1);
            }
        }
        
        if (GameState.allowPlayerAction) {
            var xMovement = 0.0f;
            var yMovement = 0.0f;

            if (Math.Abs(_playerBody.velocity.x) < maxSpeed) {
                xMovement = speed * _horizontalDirection;
            }
            if (Math.Abs(_playerBody.velocity.y) < maxSpeed) {
                yMovement = speed * _verticalDirection;
            }

            var movement = new Vector2(xMovement, yMovement);
            if (!disableInputs) {
                _playerBody.AddForce(movement);
            }
        }

        var bounds = _playerSprite.bounds;
        var bottomLeft = bounds.min;
        var bottomRight = bounds.max;
        bottomRight.y = bottomLeft.y;
        // bottomLeft = transform.TransformPoint(bottomLeft);
        // bottomRight = transform.TransformPoint(bottomRight);
        
        _bottomLeft = bottomLeft;
        _bottomRight = bottomRight;

        // Debug.Log("POS " + transform.position);
        var inTileMap = InTileMap(bottomLeft) || InTileMap(bottomRight);
        if (!inTileMap && !_charging) {
            StartFalling();
        }
    }

    private void StartFalling() {
        _playerBody.gravityScale = gameConstants.gravityScale;
        _lastFallTime = Time.time;
        _lastFallDamageTime = _lastFallTime;
        _playerSprite.sortingLayerID = _fallingSortingLayerID;
        SetColliderEnabled(false);
    }

    private static Vector2 GetMouseDirection() {
        // Get the position of the mouse click relative to the center of the screen
        var mousePosition = Mouse.current.position.ReadValue();
        var center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        var mouseOffset = mousePosition - center;
        var mouseDirection = mouseOffset.normalized;
        return mouseDirection;
    }
    
    public void OnMouseClick(InputAction.CallbackContext context) {
        if (disableInputs) { return; }
        if (!GameState.allowPlayerAction) { return; }
        // Check if the mouse button was pressed
        if (!context.started) { return; }
        
        var direction = GetMouseDirection();
        var direction3 = new Vector3(
            direction.x, direction.y, 0.0f
        );

        var angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        var rotation = Quaternion.Euler(0, 0, angle);
        
        var x = Instantiate(
            attackPrefab, 
            transform.position + direction3 * fireBallOffset, 
            rotation
        );
        // Get the Rigidbody component of the instantiated object
        var rb = x.GetComponent<Rigidbody2D>();
        // Check if the Rigidbody component exists
        if (rb == null) { return; }

        // Apply a rightward impulse force to the object
        rb.AddForce(direction * impulseForce, ForceMode2D.Impulse);
        health.Decrement(2, 0);
        playerHealthUpdate.Invoke();
    }

    public void OnChargeAttack(InputAction.CallbackContext context) {
        if (!GameState.allowPlayerAction) { return; }
        if (!context.started) { return; }
        if (_charging) { return; }

        var timestamp = Time.time;
        var timeSinceLastCharge = timestamp - _lastCharge;
        var chargeCooldownComplete = timeSinceLastCharge > chargeWaitDuration;
        if (!chargeCooldownComplete) { return; }

        _lastCharge = Time.time;
        var chargeDirection = GetMouseDirection();
        _playerBody.AddForce(chargeForce * chargeDirection, ForceMode2D.Impulse);
        
        playerAnimator.SetTrigger(CHARGING);
        health.Decrement(1, 0);
        playerHealthUpdate.Invoke();
        _charging = true;
    }

    public void EndCharge() {
        _charging = false;
    }

    private bool CanMove() {
        return GameState.allowPlayerAction && !_charging && !IsFalling();
    }

    public void OnHorizontalMoveAction(InputAction.CallbackContext context) {
        if (context.canceled) {
            _horizontalDirection = 0;
        } else if (context.started && CanMove()) {
            var faceRight = context.ReadValue<float>() > 0 ? 1 : -1;
            this._faceRight = faceRight == 1;
            _playerSprite.flipX = !this._faceRight;
            _horizontalDirection = faceRight;
        }
    }
    
    public void OnVerticalMoveAction(InputAction.CallbackContext context) {
        if (context.canceled) {
            _verticalDirection = 0;
        } else if (context.started && CanMove()) {
            var faceTop = context.ReadValue<float>() > 0 ? 1 : -1;
            _verticalDirection = faceTop;
        }
    }

    // Update is called once per frame
    private void Update() {
        var velocity = Mathf.Abs(_playerBody.velocity.magnitude);
        playerAnimator.SetFloat(SPEED, velocity);
    }

    public void DrainHealth(int amount) {
        // drain player health by 1
        health.Decrement(amount, 0);
        playerHealthUpdate.Invoke();
    }

    private void OnCollisionEnter2D(Collision2D other) {
        var enemyController = other.gameObject.GetComponent<IBaseEnemyControllerable>();
        if (enemyController == null) {
            return;
        }

        var enemyHealth = enemyController.GetEnemyHealth();
            
        if (_charging) {
            // charging the enemy increases player health by 1
            ApplyChargeAttack(enemyController);
        } else if (enemyHealth == 0) {
            // increase health after collecting dead slime
            health.Increment(gameConstants.slimeBallHealth, _maxHealth);
            gameScore.Increment();
            playerHealthUpdate.Invoke();
            scoreUpdate.Invoke();
        } else {
            // Debug.Log("ATTACK_DAMAGE: " + enemyController.GetAttackDamage());
            ReceiveSlimeDamage(enemyController.GetAttackDamage());
        }
    }

    private void ApplyChargeAttack(IBaseEnemyControllerable slimeController) {
        if (disableInputs) { return; }
        
        var enemyBody = slimeController.enemyBody;
        var chargeAlignment = Vector2.Dot(_playerBody.velocity, enemyBody.velocity);
        // whether or not player and enemy are moving towards each other
        var velocitiesColliding = chargeAlignment < 0.0f;
        
        // direction from player to enemy
        var directionToEnemy = (enemyBody.position - _playerBody.position);
        // whether player is charging in the direction of the enemy
        var chargingInEnemyDirection = (Vector2.Dot(
            directionToEnemy, _playerBody.velocity
        ) > 0.0f);

        /*
         Don't apply damage to slimes if neither of these are satisfied
         1. slime is in the direction of movement of player
         2. slime and player and moving towards each other  
        */
        if (!velocitiesColliding && !chargingInEnemyDirection) { return; }
        
        // charging the enemy increases player health by 1
        slimeController.Damage(2);
        gameScore.Increment();
        health.Increment(1, _maxHealth);
                
        var newEnemyHealth = slimeController.GetEnemyHealth();

        if (newEnemyHealth == 0) {
            // if charging killed the slime,
            // automatically destroy its sprite also
            slimeController.AttemptSelfDestruct();
            health.Increment(14, _maxHealth);
        }
                
        playerHealthUpdate.Invoke();
    }

    private void ReceiveSlimeDamage(int amount) {
        health.Decrement(amount, 0);
        _lastDamagedTime = Time.time;
        playerHealthUpdate.Invoke();
    }
    
    private static List<Vector3Int> GetAllTilePositions(Tilemap tileMap) {
        var tilePositions = new List<Vector3Int>();

        for (var x = tileMap.cellBounds.xMin; x < tileMap.cellBounds.xMax; x++) {
            for (var y = tileMap.cellBounds.yMin; y < tileMap.cellBounds.yMax; y++) {
                var localPlace = new Vector3Int(
                    x, y, (int) tileMap.transform.position.z
                );
                if (tileMap.HasTile(localPlace)) {
                    tilePositions.Add(localPlace);
                    // Debug.Log("CELL-POS " + localPlace);
                }
            }
        }

        return tilePositions;
    }
    
    private bool InTileMap(Vector3 position) {
        position.z = 0.0f;
        
        // check if the spawn position is in spawnable tile area
        var cellPosition = tileMap.WorldToCell(position);
        Debug.Log("CELL-POS " + cellPosition);
        var inTileMap = tileMap.HasTile(cellPosition);
        return inTileMap;
    }
}
