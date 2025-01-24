using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UISpriteAnimation : MonoBehaviour {
    public Image image;
    public Sprite[] spriteArray;
    public float speed = .02f;

    private int _indexSprite;
    private Coroutine _coroutineAnim;
    private bool _isDone;
    
    public void StartAnimation() {
        _isDone = false;
        StartCoroutine(PlayAnimation());
    }

    public void StopAnimation() {
        _isDone = true;
        StopCoroutine(PlayAnimation());
    }

    private IEnumerator PlayAnimation() {
        yield return new WaitForSeconds(speed);
        if (_indexSprite >= spriteArray.Length) {
            _indexSprite = 0;
        }
        
        image.sprite = spriteArray[_indexSprite];
        _indexSprite += 1;
        if (_isDone == false) {
            _coroutineAnim = StartCoroutine(PlayAnimation());
        }
    }
}