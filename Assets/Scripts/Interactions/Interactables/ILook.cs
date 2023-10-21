using UnityEngine;

namespace Interactables
{
    public interface ILook
    {
        Vector3 LookOriginPoint { get; }
        Vector3 LookDirection { get; }
    }
}