using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "BoolVariable", menuName = "ScriptableObjects/BoolVariable", order = 3)]
    public class BoolVariable: Variable<bool> {
        [SerializeField]
        public bool previousValue = false;
        public delegate void ValueChangeCallback(bool prevValue, bool newValue);
        private readonly List<ValueChangeCallback> _callbacks = new();

    
        public override void SetValue(bool targetValue) {
            var prevValue = hiddenValue;
            hiddenValue = targetValue;
            previousValue = targetValue;
            TriggerCallbacks(prevValue, targetValue);
            Debug.Log("POST+SET[1] " + previousValue);
            Debug.Log("POST+SET[2] " + hiddenValue);
        }
        
        public void LoadFromPreviousValue() {
            Debug.Log("PRELOAD " + previousValue);
            hiddenValue = previousValue;
        }

        public void ClearCallbacks() {
            _callbacks.Clear();
        }

        private void TriggerCallbacks(bool prevValue, bool newValue) {
            if (prevValue == newValue) { return; }
            foreach (var callback in _callbacks) {
                callback(prevValue, newValue);
            }
        }

        public void AttachCallback(ValueChangeCallback callback) {
            // Debug.Log("ATTACHED");
            _callbacks.Add(callback);
        }

        // overload
        public void SetValue(BoolVariable value) {
            SetValue(value.value);
        }
    
        public void SetFalse() {
            SetValue(false);
        }
    }
}