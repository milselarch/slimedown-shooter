using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class BlasterShotController : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] private float scaleSpeed = 1.0f;

    void Start() {
        StartCoroutine(ScaleAndDestroyCoroutine());
        GetComponent<AudioSource>().Play();
    }

    private IEnumerator ScaleAndDestroyCoroutine() {
        // Wait for 2 seconds
        yield return new WaitForSecondsRealtime(2);
        // Gradually scale down the GameObject
        while (transform.localScale.x > 0.01f) {
            transform.localScale -= Vector3.one * (scaleSpeed * Time.deltaTime);
            yield return null;
        }

        // Ensure the GameObject is completely scaled down
        transform.localScale = Vector3.zero;

        // Destroy the GameObject
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Enemy")) {
			collision.gameObject.GetComponent<EnemyController>().Damage();
        }
        
        // destroy self
        Destroy(gameObject);
    }
}