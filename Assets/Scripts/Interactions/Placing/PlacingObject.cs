using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(Rigidbody))]
    public class PlacingObject : MonoBehaviour, ILiftable
    {
        public event Action<bool> OnPlacedStateChanged;
        public event Action<bool> OnLiftStateChanged;

        [SerializeField] private int liftedtLayer = 10;
        [SerializeField] private LayerMask selectLayer;
        [SerializeField] private float minPlaceDist = 2;
        [SerializeField] private float maxPlaceDist = 10;
        [SerializeField] private float positionLerpMultiplier = 10;
        [SerializeField] private float rotationLerpMultiplier = 10;
        [SerializeField] private Transform downPosition;
        [SerializeField] private bool freezeUntilPickedUp;

        private Rigidbody myRigidbody;
        private readonly List<(GameObject, int)> defaultLayers = new();
        private ILiftableHolder holder;
        private ILook holderLook;
        private Coroutine positionUpdate;
        private PlacingSurface currentSurface;
        
        private bool IsLifted => holder != null;
        public bool IsPlaced { get; protected set; } = false;
        public bool IsEnabled { get; set; } = true;

        protected virtual void Awake()
        {
            myRigidbody = GetComponent<Rigidbody>();
            if (freezeUntilPickedUp)
                myRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        public virtual void PickUp(ILiftableHolder holder)
        {
            if (IsLifted)
                return;

            this.holder = holder;
            (this.holder as IInteracter).OnInteractionChanged += OnInteractionChanged;
            holderLook = holder as ILook;

            // save layers
            defaultLayers.Clear();
            foreach (Collider col in gameObject.GetComponentsInChildren<Collider>())
                defaultLayers.Add((col.gameObject, col.gameObject.layer));

            // set
            myRigidbody.useGravity = false;
            myRigidbody.isKinematic = true;
            if(freezeUntilPickedUp)
                myRigidbody.constraints = RigidbodyConstraints.None;
            foreach ((GameObject obj, int _) in defaultLayers)
                obj.layer = liftedtLayer;

            if (IsPlaced)
            {
                OnPlacedStateChanged?.Invoke(false);
                IsPlaced = false;
            }
            OnLiftStateChanged?.Invoke(true);

            positionUpdate = StartCoroutine(UpdatePosition());
        }
        public virtual void Drop()
        {
            if (!IsLifted)
                return;

            (holder as IInteracter).OnInteractionChanged -= OnInteractionChanged;

            myRigidbody.useGravity = true;
            myRigidbody.isKinematic = false;
            foreach ((GameObject obj, int defaultLayer) in defaultLayers)
                obj.layer = defaultLayer;

            StopCoroutine(positionUpdate);

            OnLiftStateChanged?.Invoke(false);

            holder = null;
        }

        protected virtual void OnInteractionChanged(bool isInteractiong)
        {
            if (isInteractiong)
                TryPlace();
        }

        protected virtual void TryPlace()
        {
            if (GetSurface(out var hit, out _) && CanPlace(hit))
            {
                holder.DropObject(this);
                myRigidbody.isKinematic = true;
                IsPlaced = true;
                OnPlacedStateChanged?.Invoke(true);
            }
            else
            {
                holder.DropObject(this);
            }
        }

        private IEnumerator UpdatePosition()
        {
            Vector3 lastGoodPos = transform.position;
            Quaternion lastGoodRot = transform.rotation;
            while (enabled)
            {
                Vector3 targetPosition = holder.GripPoint.position;
                Quaternion targetRotation = holder.GripPoint.rotation;
                if (GetSurface(out var hit, out currentSurface))
                {
                    if (CanPlace(hit))
                    {
                        Debug.DrawLine(hit.point, hit.point + hit .normal * 2, Color.green);
                        var offset = transform.position - downPosition.position;
                        targetPosition = hit.point + offset;
                        targetRotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
                    }
                    else
                    {
                        Debug.DrawLine(hit.point, hit.point + hit.normal * 2, Color.red);
                        targetPosition = lastGoodPos;
                        targetRotation = lastGoodRot;
                    }
                }

                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * positionLerpMultiplier);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationLerpMultiplier);

                lastGoodPos = transform.position;
                lastGoodRot = transform.rotation;

                yield return null;
            }
        }

        private bool GetSurface(out RaycastHit hit, out PlacingSurface surface)
        {
            surface = null;
            if (!Physics.Raycast(holderLook.LookOriginPoint, holderLook.LookDirection, out hit, maxPlaceDist, selectLayer))
                return false;

            return hit.collider.TryGetComponent(out surface);
        }
        private bool CanPlace(RaycastHit hit)
        {
            var dist = Vector3.Distance(hit.point, holder.GripPoint.position);
            if (dist > maxPlaceDist || dist < minPlaceDist)
                return false;

            return true;
        }
    }
}
