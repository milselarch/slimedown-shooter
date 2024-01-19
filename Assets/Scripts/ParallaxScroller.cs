using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ParallaxScroller : MonoBehaviour {
    public Renderer[] layers;
    public float[] speedMultiplier;
    public float scrollSpeedScale = 1.0f;
    public Transform player;
    public Transform mainCamera;
    
    private float _prevXPositionPlayer;
    private float _prevXPositionCamera;
    private float[] _offset;

    private void Start() {
        _offset = new  float[layers.Length];
        for (var i = 0; i < layers.Length; i++) {
            _offset[i] = 0.0f;	
        }
        
        _prevXPositionPlayer = player.transform.position.x;
        _prevXPositionCamera = mainCamera.transform.position.x;
    }

    private void Update() {
        // if camera has moved
        if (Mathf.Abs(
            _prevXPositionCamera - mainCamera.transform.position.x
        ) > 0.001f) {
            for (var i = 0; i < layers.Length; i++) {
                if (_offset[i] > 1.0f || _offset[i] < -1.0f) {
                    _offset[i] = 0.0f; //reset offset
                }

                var newOffset = player.transform.position.x - _prevXPositionPlayer;
                _offset[i] += newOffset * speedMultiplier[i] * scrollSpeedScale;
                layers[i].material.mainTextureOffset = new  Vector2(_offset[i], 0);
            }
        }
        
        //update previous pos
        _prevXPositionPlayer = player.transform.position.x;
        _prevXPositionCamera = mainCamera.transform.position.x;
    }
}