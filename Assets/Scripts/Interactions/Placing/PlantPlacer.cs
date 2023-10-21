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
        private TowerAI towerAI;
        private CombatEntity combatEntity;

        private void Awake()
        {
            placingObject = GetComponent<PlacingObject>();
            towerAI = GetComponent<TowerAI>();
            combatEntity = GetComponent<CombatEntity>();
            pot.SetActive(false);
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
                combatEntity.enabled = true;
                towerAI.enabled = true;
            }
            else
            {
                combatEntity.enabled = false;
                towerAI.enabled = false;
            }
        }
    }
}