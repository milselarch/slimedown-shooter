using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Serialization;

public class ParallaxScroller : MonoBehaviour {
    private const float THRESHOLD = 0.001f;
    
    public Renderer[] layers;
    public float[] speedMultiplier;
    // parallax scroll speed for when player is moving
    public float scrollSpeedScale = 1.0f;
    // parallax scroll speed based on time passed
    public float xTimeSpeedScale = 0.005f;
    public float yTimeSpeedScale = 0.00f;

    public Transform player;
    public Transform mainCamera;

    private float _startTime;
    private Vector3 _startPlayerPosition;
    private Vector2[] _offset;

    private void Start() {
        _offset = new Vector2[layers.Length];
        for (var i = 0; i < layers.Length; i++) {
            _offset[i] = Vector2.zero;	
        }
        
        Restart();
    }

    private void Restart() {
        _startTime = Time.time;
        _startPlayerPosition = GetPlayerPosition();
    }

    private Vector3 GetPlayerPosition() {
        return player.transform.position;
    }
    

    private static Vector2 ToVector2(Vector3 vector) {
        return new Vector2(vector.x, vector.y);
    }

    private void Update() {
        for (var i = 0; i < layers.Length; i++) {
            var newOffset = GetPlayerPosition() - _startPlayerPosition;
            var timePassed = Time.time - this._startTime;
            var displacement = speedMultiplier[i] * (
                scrollSpeedScale * ToVector2(newOffset) + 
                new Vector2(timePassed * xTimeSpeedScale, timePassed * yTimeSpeedScale)
            );
            
            _offset[i] = new Vector2(displacement.x % 1.0f, displacement.y % 1.0f);
            layers[i].material.mainTextureOffset = _offset[i];
        }
    }
}