using System;

namespace Interactables
{
    public interface IInteracter
    {
        event Action<Interactable> OnSelectcionChanged;
        event Action<bool> OnInteractionChanged;
        Interactable SelectedObject { get; }
        void DeselectObject(Interactable interactable);
    }
}
