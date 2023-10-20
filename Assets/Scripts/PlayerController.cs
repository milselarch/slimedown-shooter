using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerController : MonoBehaviour {
    float maxSpeed = 20;
    float speed = 10;

    private int horizontalDirection = 0;
    private int verticalDirection = 0;
    
    // keep track of which side the mario sprite is facing
    private SpriteRenderer marioSprite;
    private Rigidbody2D marioBody;

    // Start is called before the first frame update
    void Start() {
        // Set to be 30 FPS
        Application.targetFrameRate =  30;
        
        // assign mario sprite object
        marioSprite = GetComponent<SpriteRenderer>();
        marioBody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        float xMovement = 0.0f;
        float yMovement = 0.0f;
        
        if (Math.Abs(marioBody.velocity.x) < maxSpeed)
        {
            xMovement = this.speed * this.horizontalDirection;
        } 
        if (Math.Abs(marioBody.velocity.y) < maxSpeed)
        {
            yMovement = this.speed * this.verticalDirection;
        }
        
        Vector2 movement = new Vector2(xMovement, yMovement);
        marioBody.AddForce(movement);
    }

    public void OnHorizontalMoveAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int faceRight = context.ReadValue<float>() > 0 ? 1 : -1;
            horizontalDirection = faceRight;
        }
        if (context.canceled)
        {
            horizontalDirection = 0;
        }
    }
    
    public void OnVerticalMoveAction(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            int faceTop = context.ReadValue<float>() > 0 ? 1 : -1;
            verticalDirection = faceTop;
        }
        if (context.canceled)
        {
            verticalDirection = 0;
        }
    }

    // Update is called once per frame
    void Update() {
        
    }
}
