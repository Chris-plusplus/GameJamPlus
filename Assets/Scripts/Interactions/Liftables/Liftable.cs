using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(Rigidbody))]
    [DisallowMultipleComponent]
    public class Liftable : MonoBehaviour, ILiftable
    {
        public event Action<bool> OnLiftStateChanged;

        [SerializeField] private int liftedtLayer = 10;
        [SerializeField, Min(1)] private float liftForce = 10f;
        [SerializeField, Range(0f, 90f)] private float heldClamXRotation = 45f;
        [SerializeField] private Vector3 liftDirectionOffset = Vector3.zero;
        [SerializeField] private Vector3 gripPositionOffset = Vector3.zero;
        [field: SerializeField, ReadOnly] public bool IsLifted { get; protected set; } = false;

        protected Rigidbody myRigidbody;
        private readonly List<(GameObject, int)> defaultLayers = new();

        public ILiftableHolder Holder { get; protected set; }
        public bool IsEnabled => enabled;

        protected virtual void Awake()
        {
            myRigidbody = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            if (IsLifted)
                UpdateHeldObjectPosition();
        }

        public virtual void PickUp(ILiftableHolder holder)
        {
            if (IsLifted)
                return;

            Holder = holder;
            (Holder as IInteracter).OnInteractionChanged += OnInteractionChanged;

            // save layers
            defaultLayers.Clear();
            foreach (Collider col in gameObject.GetComponentsInChildren<Collider>())
                defaultLayers.Add((col.gameObject, col.gameObject.layer));

            // set
            myRigidbody.useGravity = false;
            myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            foreach ((GameObject obj, int _) in defaultLayers)
                obj.layer = liftedtLayer;

            IsLifted = true;
            OnLiftStateChanged?.Invoke(true);
        }
        public virtual void Drop()
        {
            if (!IsLifted)
                return;

            (Holder as IInteracter).OnInteractionChanged -= OnInteractionChanged;

            myRigidbody.useGravity = true;
            myRigidbody.interpolation = RigidbodyInterpolation.None;
            foreach ((GameObject obj, int defaultLayer) in defaultLayers)
                obj.layer = defaultLayer;

            IsLifted = false;
            OnLiftStateChanged?.Invoke(false);
        }

        protected virtual void OnInteractionChanged(bool isInteractiong)
        {
            if (isInteractiong)
                Holder.DropObject(this);
        }

        private void UpdateHeldObjectPosition()
        {
            myRigidbody.velocity = (Holder.GripPoint.position + (transform.TransformVector(gripPositionOffset)) - transform.position) * liftForce + Holder.Velocity;

            Vector3 handRot = Holder.GripPoint.rotation.eulerAngles;
            if (handRot.x > 180f)
                handRot.x -= 360f;
            handRot.x = Mathf.Clamp(handRot.x, -heldClamXRotation, heldClamXRotation);
            transform.rotation = Quaternion.Euler(handRot + liftDirectionOffset);
        }

        private void OnDestroy()
        {
            if (IsLifted)
                Holder.DropObject(this);
        }
    }
}