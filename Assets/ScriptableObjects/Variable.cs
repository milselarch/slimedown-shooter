using UnityEngine;
using UnityEngine.Serialization;

namespace ScriptableObjects {
    public abstract class Variable<T> : ScriptableObject
    {
#if UNITY_EDITOR
        [FormerlySerializedAs("DeveloperDescription")] [Multiline]
        public string developerDescription = "";
#endif
        [SerializeField]
        protected T hiddenValue;
        public T value
        {
            get => hiddenValue;
            set => SetValue(value);
        }

        public abstract void SetValue(T value);
    }
}