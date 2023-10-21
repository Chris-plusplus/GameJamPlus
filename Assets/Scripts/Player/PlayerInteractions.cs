using UnityEngine;
using NaughtyAttributes;
using System;

namespace Interactables
{
    public class PlayerInteractions : MonoBehaviour, ILiftableHolder
    {
        public event Action<Interactable> OnSelectcionChanged;
        public event Action<bool> OnInteractionChanged;

        [Header("Select")]
        [SerializeField, Required] private Transform playerCamera;
        [SerializeField, Required] private CharacterController characterController;
        [SerializeField] private float selectRange = 10f;
        [SerializeField] private LayerMask selectLayer;
        [field: SerializeField, ReadOnly] public Interactable SelectedObject { get; private set; } = null;

        [Header("Hold")]
        [SerializeField, Required] private Transform handTransform;
        [field: SerializeField, ReadOnly] public ILiftable HeldObject { get; private set; } = null;

        [field: Header("Input")]
        [field: SerializeField, ReadOnly] public bool Interacting { get; private set; } = false;


        public Transform GripPoint => handTransform;
        public Vector3 Velocity => characterController.velocity;

        private void OnEnable()
        {
            OnInteractionChanged += UpdateOnInteraction;
        }
        private void OnDisable()
        {
            OnInteractionChanged -= UpdateOnInteraction;
        }

        private void Update()
        {
            UpdateInput();
            UpdateSelectedObject();
        }

        private void UpdateInput()
        {
            bool interacting = Input.GetMouseButton(0);
            if (interacting != Interacting)
            {
                Interacting = interacting;
                OnInteractionChanged?.Invoke(interacting); 
            }
        }
        private void UpdateSelectedObject()
        {
            Interactable foundInteractable = null;
            if (Physics.SphereCast(playerCamera.position, 0.2f, playerCamera.forward, out RaycastHit hit, selectRange, selectLayer))
                foundInteractable = hit.collider.GetComponent<Interactable>();

            if (SelectedObject == foundInteractable)
                return;

            if (SelectedObject)
            {
                SelectedObject.Deselect();
                OnSelectcionChanged?.Invoke(null);
            }

            SelectedObject = foundInteractable;

            if (foundInteractable && foundInteractable.enabled)
            {

                foundInteractable.Select();
                OnSelectcionChanged?.Invoke(foundInteractable);
            }
        }

        private void UpdateOnInteraction(bool isInteractiong)
        {
            if (isInteractiong)
            {
                if (SelectedObject != null)
                {
                    SelectedObject.Interact(Interacting);
                }

                ChangeHeldObject();
            }
        }


        private void ChangeHeldObject()
        {
            if (HeldObject == null && SelectedObject != null && SelectedObject.TryGetComponent(out Liftable liftable))
                PickUpObject(liftable);
        }
        private void PickUpObject(ILiftable obj)
        {
            if (obj == null)
            {
                Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to pick up null object!");
                return;
            }

            HeldObject = obj;
            obj.PickUp(this);
        }
        public void DropObject(ILiftable obj)
        {
            if (obj == null)
            {
                Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to drop null object!");
                return;
            }

            if (obj != HeldObject)
            {
                Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to drop object that is not currentyl holded!");
                return;
            }

            HeldObject = null;
            obj.Drop();
        }
    }
}