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

        private Rigidbody rbody;
        private ILiftableHolder liftedHolder;
        private readonly List<(GameObject, int)> defaultLayers = new();

        public bool IsLifted => liftedHolder != null;

        protected virtual void Awake()
        {
            rbody = GetComponent<Rigidbody>();
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

            // save layers
            defaultLayers.Clear();
            foreach (Collider col in gameObject.GetComponentsInChildren<Collider>())
                defaultLayers.Add((col.gameObject, col.gameObject.layer));

            // set
            rbody.useGravity = false;
            rbody.interpolation = RigidbodyInterpolation.Interpolate;
            foreach ((GameObject obj, int defaultLayer) item in defaultLayers)
                item.obj.layer = liftedtLayer;

            OnLiftStateChanged?.Invoke(true);
        }
        public virtual void Drop()
        {
            if (!IsLifted)
                return;

            rbody.useGravity = true;
            rbody.interpolation = RigidbodyInterpolation.None;
            foreach ((GameObject obj, int defaultLayer) item in defaultLayers)
                item.obj.layer = item.defaultLayer;

            liftedHolder = null;
            OnLiftStateChanged?.Invoke(false);
        }

        private void UpdateHeldObjectPosition()
        {
            rbody.velocity = (liftedHolder.GripPoint.position - transform.position) * liftForce + liftedHolder.Velocity;

            Vector3 handRot = liftedHolder.GripPoint.rotation.eulerAngles;
            if (handRot.x > 180f)
                handRot.x -= 360f;
            handRot.x = Mathf.Clamp(handRot.x, -heldClamXRotation, heldClamXRotation);
            transform.rotation = Quaternion.Euler(handRot + liftDirectionOffset);
        }
    }
}