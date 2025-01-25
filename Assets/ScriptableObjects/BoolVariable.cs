using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "BoolVariable", menuName = "ScriptableObjects/BoolVariable", order = 3)]
    public class BoolVariable: Variable<bool> {
        public bool previousValue;
        public delegate void ValueChangeCallback(bool prevValue, bool newValue);
        private readonly List<ValueChangeCallback> _callbacks = new();

    
        public override void SetValue(bool value) {
            _value = value;
            TriggerCallbacks(previousValue, _value);
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
            Debug.Log("ATTACHED");
            _callbacks.Add(callback);
        }

        // overload
        public void SetValue(BoolVariable value) {
            SetValue(value.Value);
        }
    
        public void SetFalse() {
            previousValue = false;
        }
    }
}