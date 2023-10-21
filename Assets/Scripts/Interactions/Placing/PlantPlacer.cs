using Combat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(PlacingObject))]
    public class PlantPlacer : MonoBehaviour
    {
        [SerializeField] private GameObject pot;

        private PlacingObject placingObject;

        private void Awake()
        {
            placingObject = GetComponent<PlacingObject>();
            pot.SetActive(false);
            UpdateAIState();
        }

        private void OnEnable()
        {
            placingObject.OnLiftStateChanged += OnLiftStateChanged;
            placingObject.OnPlacedStateChanged += OnPlacedStateChanged;
        }
        private void OnDisable()
        {
            placingObject.OnLiftStateChanged -= OnLiftStateChanged;
            placingObject.OnPlacedStateChanged -= OnPlacedStateChanged;
        }

        private void OnLiftStateChanged(bool isLifted)
        {
            pot.SetActive(!isLifted);
        }
        private void OnPlacedStateChanged(bool isPlaced)
        {
            if (isPlaced)
            {
                pot.SetActive(false);
            }

            UpdateAIState();
        }

        private void UpdateAIState()
        {
            foreach (var module in GetComponents<IAIModule>())
            {
                module.SetActive(placingObject.IsPlaced);
            }
        }
    }
}