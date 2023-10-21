using QuickOutline;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(Interactable))]
    [DisallowMultipleComponent]
    public class InteractionHighlightController : MonoBehaviour
    {
        [SerializeField] private Outline[] outlines;
        private Interactable interactable;

        private void Awake()
        {
            interactable = GetComponent<Interactable>();
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

        public void UpdateOutline(bool active)
        {
            foreach (var outline in outlines)
                outline.enabled = active;
        }
    }
}