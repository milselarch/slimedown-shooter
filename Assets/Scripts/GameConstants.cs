using UnityEngine;

[CreateAssetMenu(
    fileName = "GameConstants", 
    menuName = "ScriptableObjects/GameConstants", order = 1
)]
public  class GameConstants : ScriptableObject {
    // set your data here
    public int startMaxHealth = 100;
    public float gravityScale = 3.0f;
    public float minYCameraPosition = -100.0f;
}
