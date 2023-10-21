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
        [field: SerializeField, ReadOnly] public bool IsLifted { get; private set; } = false;

        protected Rigidbody myRigidbody;
        protected ILiftableHolder liftedHolder;
        private readonly List<(GameObject, int)> defaultLayers = new();

        public ILiftableHolder LiftableHolder => liftedHolder;

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

            liftedHolder = holder;
            liftedHolder.OnInteractionChanged += OnInteractionChanged;

            // save layers
            defaultLayers.Clear();
            foreach (Collider col in gameObject.GetComponentsInChildren<Collider>())
                defaultLayers.Add((col.gameObject, col.gameObject.layer));

            // set
            myRigidbody.useGravity = false;
            myRigidbody.interpolation = RigidbodyInterpolation.Interpolate;
            foreach ((GameObject obj, int defaultLayer) item in defaultLayers)
                item.obj.layer = liftedtLayer;

            IsLifted = true;
            OnLiftStateChanged?.Invoke(true);
        }
        public virtual void Drop()
        {
            if (!IsLifted)
                return;

            liftedHolder.OnInteractionChanged -= OnInteractionChanged;

            myRigidbody.useGravity = true;
            myRigidbody.interpolation = RigidbodyInterpolation.None;
            foreach ((GameObject obj, int defaultLayer) item in defaultLayers)
                item.obj.layer = item.defaultLayer;

            IsLifted = false;
            OnLiftStateChanged?.Invoke(false);
        }

        protected virtual void OnInteractionChanged(bool isInteractiong)
        {
            if (isInteractiong)
                liftedHolder.DropObject(this);
        }

        private void UpdateHeldObjectPosition()
        {
            myRigidbody.velocity = (liftedHolder.GripPoint.position - transform.position) * liftForce + liftedHolder.Velocity;

            Vector3 handRot = liftedHolder.GripPoint.rotation.eulerAngles;
            if (handRot.x > 180f)
                handRot.x -= 360f;
            handRot.x = Mathf.Clamp(handRot.x, -heldClamXRotation, heldClamXRotation);
            transform.rotation = Quaternion.Euler(handRot + liftDirectionOffset);
        }
    }
}