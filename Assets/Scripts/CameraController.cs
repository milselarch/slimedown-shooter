using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
    public Transform player; // Players's Transform

    private float offsetX; // initial x-offset between camera and Mario
    private float startX; // smallest x-coordinate of the Camera
    private float endX; // largest x-coordinate of the camera
    
    private float offsetY; // initial y-offset between camera and Mario
    private float startY; // smallest y-coordinate of the Camera
    private float endY; // largest y-coordinate of the camera
    
    private float viewportHalfWidth;
    private float viewportHalfHeight;

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
        
        // get coordinate of the bottomleft of the viewport
        // z doesn't matter since the camera is orthographic
        Vector3 bottomLeft = Camera.main.ViewportToWorldPoint(
            new Vector3(0, 0, 0)
        );
        viewportHalfWidth = Mathf.Abs(
            bottomLeft.x - this.transform.position.x
        );
        viewportHalfHeight = Mathf.Abs(
            bottomLeft.y - this.transform.position.y
        );

        offsetX = this.transform.position.x - player.position.x;
        offsetY = this.transform.position.y - player.position.y;
    }
    
    public void GameRestart() {
        // reset camera position
        transform.position = startPosition;
    }

    void Update() {
        // Debug.Log("POS_X: " + player.position.x);
        float desiredX = player.position.x + offsetX;
        float desiredY = player.position.y + offsetY;
        
        // Debug.Log("IN_OF_BOUNDS");
        this.transform.position = new Vector3(
            desiredX, desiredY, this.transform.position.z
        );
    }
}