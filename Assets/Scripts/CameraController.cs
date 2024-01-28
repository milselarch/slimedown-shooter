using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform player; // Player's Transform
    public GameConstants gameConstants;
    
    private float _offsetX; // initial x-offset between camera and Mario
    private float _startX; // smallest x-coordinate of the Camera
    private float _endX; // largest x-coordinate of the camera
    
    private float _offsetY; // initial y-offset between camera and Mario
    private float _startY; // smallest y-coordinate of the Camera
    private float _endY; // largest y-coordinate of the camera
    
    private float _viewportHalfWidth;
    private float _viewportHalfHeight;

    private Vector3 _startPosition;

    private void Start() {
        var position = this.transform.position;
        _startPosition = position;
        
        // get coordinate of the bottom left of the viewport
        // z doesn't matter since the camera is orthographic
        if (Camera.main == null) {
            return;
        }

        var bottomLeft = Camera.main.ViewportToWorldPoint(
            new Vector3(0, 0, 0)
        );
        _viewportHalfWidth = Mathf.Abs(
            bottomLeft.x - position.x
        );
        _viewportHalfHeight = Mathf.Abs(
            bottomLeft.y - position.y
        );

        var playerPosition = player.position;
        _offsetX = position.x - playerPosition.x;
        _offsetY = position.y - playerPosition.y;
    }
    
    public void GameRestart() {
        // reset camera position
        transform.position = _startPosition;
    }

    private void Update() {
        // Debug.Log("POS_X: " + player.position.x);
        var position = player.position;
        var desiredX = position.x + _offsetX;
        var desiredY = Math.Max(
            position.y + _offsetY, gameConstants.minYCameraPosition
        );
        
        // Debug.Log("IN_OF_BOUNDS");
        transform.position = new Vector3(
            desiredX, desiredY, this.transform.position.z
        );
    }
}