using UnityEngine;
using NaughtyAttributes;
using System;

namespace Interactables
{
    [DisallowMultipleComponent]
    public class Interactable : MonoBehaviour
    {
        public event Action<bool> OnInteractionChanged;
        public event Action<bool> OnSelectionChanged;

        [field: SerializeField] public bool ShowPointerOnInterract { get; protected set; } = true;
        [field: SerializeField, ReadOnly] public bool IsSelected { get; protected set; }
        [field: SerializeField, ReadOnly] public bool IsInteractiong { get; protected set; }
        
        public IInteracter Interacter { get; protected set; } 

        protected virtual void Awake()
        {
            Deselect();
        }

        protected virtual void OnDisable()
        {
            Deselect();
        }

        public virtual void Select(IInteracter interacter)
        {
            Interacter = interacter;
            IsSelected = true;
            OnSelectionChanged?.Invoke(true);
        }
        public virtual void Deselect()
        {
            Interacter = null;
            IsSelected = false;
            OnSelectionChanged?.Invoke(false);
            if (IsInteractiong)
                Interact(false);
        }

        public virtual void Interact(bool isInteracting)
        {
            IsInteractiong = isInteracting;
            OnInteractionChanged?.Invoke(isInteracting);
        }
    }
}