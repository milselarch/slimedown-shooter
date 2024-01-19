using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ParallaxScroller : MonoBehaviour {
    private const float THRESHOLD = 0.001f;
    
    public Renderer[] layers;
    public float[] speedMultiplier;
    public float scrollSpeedScale = 1.0f;
    public Transform player;
    public Transform mainCamera;

    private Vector3 _startPlayerPosition;
    private Vector3 _prevCameraPosition;
    private Vector2[] _offset;

    private void Start() {
        _offset = new Vector2[layers.Length];
        for (var i = 0; i < layers.Length; i++) {
            _offset[i] = Vector2.zero;	
        }
        
        UpdatePositions();
        Restart();
    }

    private void Restart() {
        _startPlayerPosition = GetPlayerPosition();
    }

    private Vector3 GetPlayerPosition() {
        return player.transform.position;
    }

    private void UpdatePositions() {
        _prevCameraPosition = mainCamera.transform.position;
    }

    private Vector2 ToVector2(Vector3 vector) {
        return new Vector2(vector.x, vector.y);
    }

    private void Update() {
        // if camera has moved
        var offset = _prevCameraPosition - mainCamera.transform.position;
        var xOffset = offset.x;
        var yOffset = offset.y;
        
        if (Math.Max(Math.Abs(xOffset), Math.Abs(yOffset)) > THRESHOLD) {
            for (var i = 0; i < layers.Length; i++) {
                var newOffset = GetPlayerPosition() - _startPlayerPosition;
                var scale = speedMultiplier[i] * scrollSpeedScale;
                var displacement = scale * ToVector2(newOffset);
                
                _offset[i] = new Vector2(displacement.x % 1.0f, displacement.y % 1.0f);
                layers[i].material.mainTextureOffset = _offset[i];
            }
        }
        
        //update previous pos
        UpdatePositions();
    }
}