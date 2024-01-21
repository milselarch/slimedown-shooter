using UnityEngine;

public class CrosshairCursor : MonoBehaviour {
    public new Camera camera;
    public PlayerController playerController;
    private SpriteRenderer _renderer;
    public Sprite chargingSprite;

    private Sprite _defaultSprite;

    // Start is called before the first frame update
    private void Awake() {
        Cursor.visible = false;
        _renderer = GetComponent<SpriteRenderer>();
        _defaultSprite = _renderer.sprite;
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
        if (GameState.IsApproxEqual(chargeProgress, 1.0f)) {
            // charge attack is ready
            _renderer.sprite = _defaultSprite;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        } else {
            _renderer.sprite = chargingSprite;
            var rotation = 360.0f * chargeProgress;
            transform.rotation = Quaternion.Euler(0, 0, rotation);
        }
    }
}
