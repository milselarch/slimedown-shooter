using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour {
    public Image image;
    public Sprite[] spriteArray;
    public float speed = .02f;

    private int _indexSprite;
    private Coroutine _coroutineAnim;
    private bool _isDone;
    
    public void Func_PlayUIAnim() {
        _isDone = false;
        StartCoroutine(Func_PlayAnimUI());
    }

    public void Func_StopUIAnim() {
        _isDone = true;
        StopCoroutine(Func_PlayAnimUI());
    }
    
    IEnumerator Func_PlayAnimUI() {
        yield return new WaitForSeconds(speed);
        if (_indexSprite >= spriteArray.Length) {
            _indexSprite = 0;
        }
        
        image.sprite = spriteArray[_indexSprite];
        _indexSprite += 1;
        if (_isDone == false) {
            _coroutineAnim = StartCoroutine(Func_PlayAnimUI());
        }
    }
}