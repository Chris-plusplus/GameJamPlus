using NaughtyAttributes;
using QuickOutline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Interactables
{
    [RequireComponent(typeof(Interactable))]
    public class Money : MonoBehaviour
    {
        [SerializeField] private GameObject coinPrefab;
        [SerializeField] private Transform coinParent;
        [SerializeField] private float coinYSize = 0.05f;
        [SerializeField] private BoxCollider tCollider;

        [Button]
        private void AddValue() => SetValue(Value + 1);

        [Button]
        private void RemoveValue() => SetValue(Value - 1);

        private readonly List<GameObject> coins = new();
        private readonly List<Outline> outlines = new();
        private Interactable interactable;

        public int Value { get; private set; } = 0;

        private void Awake()
        {
            interactable = GetComponent<Interactable>();
            SetValue(1);
        }
        private void OnEnable()
        {
            interactable.OnSelectionChanged += Highlight;
        }
        private void OnDisable()
        {
            interactable.OnSelectionChanged -= Highlight;
        }

        public void SetValue(int value)
        {
            if (value <= 0)
            {
                Destroy(gameObject);
                return;
            }

            if (value > Value)
            {
                int toSpawn = value - Value;
                for (int i = 0; i < toSpawn; i++)
                {
                    GameObject newCoin = Instantiate(coinPrefab, coinParent);
                    newCoin.transform.localPosition = Vector3.up * coinYSize * coins.Count;
                    newCoin.transform.localRotation = Quaternion.identity;
                    coins.Add(newCoin);
                    var newOutlines = newCoin.GetComponentsInChildren<Outline>();
                    foreach (var outline in newOutlines)
                        outline.enabled = interactable.IsSelected;
                    outlines.AddRange(newOutlines);
                }
            }
            else
            {
                int toDestroy = Value - value;
                for (int i = 0; i < toDestroy; i++)
                {
                    GameObject lastCoin = coins[^1];
                    coins.Remove(lastCoin);
                    var cooinOutlines = lastCoin.GetComponentsInChildren<Outline>();
                    foreach (var outline in cooinOutlines)
                        outlines.Remove(outline);
                    Destroy(lastCoin);
                }
            }

            float height = coinYSize * (coins.Count + 1);
            tCollider.size = new Vector3(tCollider.size.x, height, tCollider.size.z);
            float offsetY = height * 0.5f - coinYSize;
            tCollider.center = new Vector3(tCollider.center.x, offsetY, tCollider.center.z);

            Value = value;
        }

        private void Highlight(bool active)
        {
            foreach (var outline in outlines)
                outline.enabled = active;
        }
    }
}