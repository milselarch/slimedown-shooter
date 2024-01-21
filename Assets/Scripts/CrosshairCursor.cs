using UnityEngine;

public class CrosshairCursor : MonoBehaviour {
    public new Camera camera;

    // Start is called before the first frame update
    private void Awake() {
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        var cursorPos = camera.ScreenToWorldPoint(Input.mousePosition);
        var crosshairTransform = transform;
        var position = crosshairTransform.position;
        position.x = cursorPos.x;
        position.y = cursorPos.y;
        crosshairTransform.position = position;
    }
}
