using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects {
    [CreateAssetMenu(fileName = "IntVariable", menuName = "ScriptableObjects/IntVariable", order = 2)]
    public class IntVariable: Variable<int> {
        public int previousHighestValue;
        public delegate void ValueChangeCallback(int prevValue, int newValue);
        private readonly List<ValueChangeCallback> _callbacks = new();

    
        public override void SetValue(int targetValue) {
            var prevValue = this.hiddenValue;
            if (targetValue > previousHighestValue) {
                previousHighestValue = targetValue;
            }

            hiddenValue = targetValue;
            TriggerCallbacks(prevValue, hiddenValue);
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
        public void SetValue(IntVariable targetVar) {
            SetValue(targetVar.value);
        }

        public void Increment() {
            this.ApplyChange(1);
        }
    
        public void Increment(int amount, int ceiling) {
            var newValue = Math.Min(ceiling, this.value + amount);
            this.SetValue(newValue);
        }

        public void Decrement(int amount, int floor) {
            var newValue = Math.Max(floor, this.value - amount);
            this.SetValue(newValue);
        }

        public void ApplyChange(int amount) {
            var prevValue = this.hiddenValue;
            this.value += amount;
            TriggerCallbacks(prevValue, hiddenValue);
        }

        public void ApplyChange(int amount, int ceilingValue) {
            var newValue = Math.Min(ceilingValue, this.value + amount);
            this.SetValue(newValue);
        }

        public void ApplyChange(IntVariable amount) {
            ApplyChange(amount.value);
        }

        public void ResetHighestValue() {
            previousHighestValue = 0;
            hiddenValue = previousHighestValue;
        }
    }
}