using UnityEngine;
using NaughtyAttributes;
using System;
using UnityEditor.Media;

namespace Interactables
{
    public class PlayerInteractions : MonoBehaviour, ILiftableHolder, IInteracter, ILook
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

        public Vector3 LookOriginPoint => playerCamera.position;
        public Vector3 LookDirection => playerCamera.forward;


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
            if (Physics.SphereCast(LookOriginPoint, 0.2f, LookDirection, out RaycastHit hit, selectRange, selectLayer))
                foundInteractable = hit.collider.GetComponent<Interactable>();

            if (SelectedObject == foundInteractable)
                return;

            if (SelectedObject != null)
                DeselectObject(SelectedObject);

            if (foundInteractable != null && foundInteractable.enabled)
                SelectObject(foundInteractable);
        }

        private void UpdateOnInteraction(bool isInteractiong)
        {
            if (SelectedObject != null)
            {
                SelectedObject.Interact(Interacting);
            }

            if (isInteractiong)
            {
                ChangeHeldObject();
            }
        }

        protected void SelectObject(Interactable obj)
        {
            if (obj == null)
            {
                Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to pick up null object!");
                return;
            }

            obj.Select(this);
            SelectedObject = obj;
            OnSelectcionChanged?.Invoke(obj);
        }
        public void DeselectObject(Interactable obj)
        {
            if (obj == null)
            {
                Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to deselect null object!");
                return;
            }

            if (obj != SelectedObject)
            {
                Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to deselect object that is not currentyl selected!");
                return;
            }

            obj.Deselect();
            SelectedObject = null;
            OnSelectcionChanged?.Invoke(null);
        }


        private void ChangeHeldObject()
        {
            if (HeldObject == null && SelectedObject != null && SelectedObject.TryGetComponent(out ILiftable liftable) && liftable.IsEnabled)
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
                Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to drop object that is not currentyl held!");
                return;
            }

            HeldObject = null;
            obj.Drop();
        }
    }
}