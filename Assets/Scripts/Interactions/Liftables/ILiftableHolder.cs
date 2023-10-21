using System;
using UnityEngine;

namespace Interactables
{
    public interface ILiftableHolder
    {
        Transform GripPoint { get; }
        Vector3 Velocity { get; }
        void DropObject(ILiftable obj);
    }
}