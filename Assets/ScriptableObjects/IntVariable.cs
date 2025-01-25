using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "IntVariable", menuName = "ScriptableObjects/IntVariable", order = 2)]
    public class IntVariable: Variable<int> {
        public int previousHighestValue;
        public delegate void ValueChangeCallback(int prevValue, int newValue);
        private readonly List<ValueChangeCallback> _callbacks = new();

    
        public override void SetValue(int value) {
            var prevValue = this._value;
            if (value > previousHighestValue) {
                previousHighestValue = value;
            }

            _value = value;
            TriggerCallbacks(prevValue, _value);
        }

        public void ClearCallbacks() {
            _callbacks.Clear();
        }

        private void TriggerCallbacks(int prevValue, int newValue) {
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
        public void SetValue(IntVariable value) {
            SetValue(value.Value);
        }

        public void Increment() {
            this.ApplyChange(1);
        }
    
        public void Increment(int amount, int ceiling) {
            var newValue = Math.Min(ceiling, this.Value + amount);
            this.SetValue(newValue);
        }

        public void Decrement(int amount, int floor) {
            var newValue = Math.Max(floor, this.Value - amount);
            this.SetValue(newValue);
        }

        public void ApplyChange(int amount) {
            var prevValue = this._value;
            this.Value += amount;
            TriggerCallbacks(prevValue, _value);
        }

        public void ApplyChange(int amount, int ceilingValue) {
            var newValue = Math.Min(ceilingValue, this.Value + amount);
            this.SetValue(newValue);
        }

        public void ApplyChange(IntVariable amount) {
            ApplyChange(amount.Value);
        }

        public void ResetHighestValue() {
            previousHighestValue = 0;
        }

    }
}