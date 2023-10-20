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

        [field: SerializeField] public bool ShowPointerOnInterract { get; private set; } = true;
        [field: SerializeField, ReadOnly] public bool IsSelected { get; private set; }
        [field: SerializeField, ReadOnly] public bool IsInteractiong { get; private set; }

        protected virtual void Awake()
        {
            Deselect();
        }

        public virtual void Select()
        {
            IsSelected = true;
            OnSelectionChanged?.Invoke(true);
        }
        public virtual void Deselect()
        {
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