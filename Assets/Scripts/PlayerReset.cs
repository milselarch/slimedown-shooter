using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerReset : MonoBehaviour {
    public UnityEvent restartEvent;
    public IntVariable playerHealth;
    private bool _resetting = false;
    
    public void ResetPlayerUponDeath() {
        // Checks the player's health and calls the restart event if 
        // the player's health is zero
        // Debug.Log("RESET_PLAYER");
        if (_resetting) { return; }
        if (playerHealth.Value > 0) { return; }
        
        _resetting = true;
        StartCoroutine(WaitThenRestart());
    }
    
    private IEnumerator WaitThenRestart() {
        // waits for a short period of time before calling the restart event
        // Debug.Log("WAITING");
        yield return new WaitForSeconds(1);
        restartEvent.Invoke();
        Debug.Log("RESTART_INVOKED");
        _resetting = false;
    }
}
