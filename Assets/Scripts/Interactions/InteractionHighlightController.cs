using QuickOutline;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(Interactable))]
    [RequireComponent(typeof(Outline))]
    [DisallowMultipleComponent]
    public class InteractionHighlightController : MonoBehaviour
    {
        private Interactable interactable;
        private Outline outline;

        private void Awake()
        {
            interactable = GetComponent<Interactable>();
            outline = GetComponent<Outline>();
            UpdateOutline(false);
        }

        private void OnEnable()
        {
            interactable.OnSelectionChanged += UpdateOutline;
        }

        private void OnDisable()
        {
            interactable.OnSelectionChanged -= UpdateOutline;
        }

        private void UpdateOutline(bool active) => outline.enabled = active;
    }
}