using System;
using UnityEngine;

namespace Interactables
{
    public interface ILiftableHolder
    {
        Transform GripPoint { get; }
        Vector3 Velocity { get; }
        Interactable SelectedObject { get; }
        void DropObject(ILiftable obj);

        public event Action<bool> OnInteractionChanged;
    }
}