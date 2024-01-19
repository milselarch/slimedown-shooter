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
    
    private Vector3 _prevPlayerPosition;
    private Vector3 _prevCameraPosition;
    private Vector2[] _offset;

    private void Start() {
        _offset = new Vector2[layers.Length];
        for (var i = 0; i < layers.Length; i++) {
            _offset[i] = Vector2.zero;	
        }
        
        UpdatePositions();
    }

    private void UpdatePositions() {
        _prevPlayerPosition = player.transform.position;
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
                if (_offset[i].x > 1.0f || _offset[i].x < -1.0f) {
                    _offset[i].x = 0.0f; // reset offset
                }
                if (_offset[i].y > 1.0f || _offset[i].y < -1.0f) {
                    _offset[i].y = 0.0f; // reset offset
                }

                var newOffset = player.transform.position - _prevPlayerPosition;
                var scale = speedMultiplier[i] * scrollSpeedScale;
                _offset[i] += scale * ToVector2(newOffset);
                layers[i].material.mainTextureOffset = _offset[i];
            }
        }
        
        //update previous pos
        UpdatePositions();
    }
}