using UnityEngine;
using NaughtyAttributes;
using System;

namespace Interactables
{
    public class PlayerInteractions : MonoBehaviour
    {
        public event Action OnSelect;
        public event Action OnDeselect;

        public event Action OnInteractionStart;
        public event Action OnInteractionEnd;

        [Header("Select")]
        [SerializeField, Required] private Transform playerCamera;
        [SerializeField, Required] private CharacterController characterController;
        [SerializeField] private float selectRange = 10f;
        [SerializeField] private LayerMask selectLayer;
        [field: SerializeField, ReadOnly] public Interactable SelectedObject { get; private set; } = null;

        [Header("Hold")]
        [SerializeField, Required] private Transform handTransform;
        [SerializeField, Min(1)] private float holdingForce = 0.5f;
        [SerializeField] private int heldObjectLayer;
        [SerializeField][Range(0f, 90f)] private float heldClamXRotation = 45f;
        [field: SerializeField, ReadOnly] public Liftable HeldObject { get; private set; } = null;

        [field: Header("Input")]
        [field: SerializeField, ReadOnly] public bool Interacting { get; private set; } = false;


        private void OnEnable()
        {
            OnInteractionStart += ChangeHeldObject;
        }
        private void OnDisable()
        {
            OnInteractionStart -= ChangeHeldObject;
        }

        private void Update()
        {
            UpdateInput();
            UpdateSelectedObject();
        }

        private void FixedUpdate()
        {
            if (HeldObject)
                UpdateHeldObjectPosition();
        }

        private void UpdateInput()
        {
            bool interacting = Input.GetMouseButton(0);
            if (interacting != Interacting)
            {
                Interacting = interacting;
                if (interacting)
                    OnInteractionStart?.Invoke();
                else
                    OnInteractionEnd?.Invoke();
                OnInteractionChanged();
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
                OnDeselect?.Invoke();
            }

            SelectedObject = foundInteractable;

            if (foundInteractable && foundInteractable.enabled)
            {

                foundInteractable.Select();
                OnSelect?.Invoke();
            }
        }

        private void OnInteractionChanged()
        {
            if (SelectedObject != null)
            {
                SelectedObject.Interact(Interacting);
            }
        }


        #region held object

        private void UpdateHeldObjectPosition()
        {
            HeldObject.Rigidbody.velocity = (handTransform.position - HeldObject.transform.position) * holdingForce + characterController.velocity;

            Vector3 handRot = handTransform.rotation.eulerAngles;
            if (handRot.x > 180f)
                handRot.x -= 360f;
            handRot.x = Mathf.Clamp(handRot.x, -heldClamXRotation, heldClamXRotation);
            HeldObject.transform.rotation = Quaternion.Euler(handRot + HeldObject.LiftDirectionOffset);
        }
        private void ChangeHeldObject()
        {
            if (HeldObject)
                DropObject(HeldObject);
            else if (SelectedObject != null && SelectedObject.TryGetComponent(out Liftable liftable))
                PickUpObject(liftable);
        }
        private void PickUpObject(Liftable obj)
        {
            if (obj == null)
            {
                Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to pick up null object!");
                return;
            }

            HeldObject = obj;
            obj.PickUp(heldObjectLayer);
        }
        private void DropObject(Liftable obj)
        {
            if (obj == null)
            {
                Debug.LogWarning($"{nameof(PlayerInteractions)}: Attempted to drop null object!");
                return;
            }

            HeldObject = null;
            obj.Drop();
        }

        private void CheckHeldObjectOnTeleport()
        {
            if (HeldObject != null)
                DropObject(HeldObject);
        }

        #endregion
    }
}