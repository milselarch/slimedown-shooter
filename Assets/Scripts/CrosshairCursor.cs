using UnityEngine;

public class CrosshairCursor : MonoBehaviour {
    public new Camera camera;
    public PlayerController playerController;
    public Sprite chargingSprite;
    public GameObject maskObject;

    private SpriteRenderer _renderer;
    private Sprite _defaultSprite;
    private float _height;

    // Start is called before the first frame update
    private void Awake() {
        Cursor.visible = false;
        _renderer = GetComponent<SpriteRenderer>();
        _defaultSprite = _renderer.sprite;
        _height = _defaultSprite.bounds.size.y;
        // maskObject.transform.SetParent(transform);
    }

    // Update is called once per frame
    private void FixedUpdate() {
        var cursorPos = camera.ScreenToWorldPoint(Input.mousePosition);
        var crosshairTransform = transform;
        var position = crosshairTransform.position;
        position.x = cursorPos.x;
        position.y = cursorPos.y;
        crosshairTransform.position = position;

        var chargeProgress = playerController.GetChargeProgress();
        Debug.Log("CHARGE_PROGRESS " + chargeProgress);
        _renderer.sprite = 
            GameState.IsApproxEqual(chargeProgress, 1.0f) ? 
            _defaultSprite : chargingSprite;
        
        // Debug.Log("PROGRESS " + chargeProgress);
        position.y -= chargeProgress * _height / 8.0f;
        maskObject.transform.position = position;
    }
}
