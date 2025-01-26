using UnityEngine;

// no arguments
namespace ScriptableObjects {
    [CreateAssetMenu(
        fileName = "SimpleGameEvent", 
        menuName = "ScriptableObjects/SimpleGameEvent", order = 3
    )]
    public class SimpleGameEvent : GameEvent<Object>
    {
        // leave empty
    }
}