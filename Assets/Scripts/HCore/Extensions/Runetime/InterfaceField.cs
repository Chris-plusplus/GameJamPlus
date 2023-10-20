using UnityEngine;

namespace HCore.Extensions
{
    [System.Serializable]
    public class InterfaceField<T> where T : class
    {
        [SerializeField] private Object objectRef;

        public bool HasValue  => objectRef != null;
        public T Value => objectRef as T;


        public static implicit operator T (InterfaceField<T> field) => field.Value;
    }
}