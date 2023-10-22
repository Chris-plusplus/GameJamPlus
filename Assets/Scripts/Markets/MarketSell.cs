using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Interactables;
using System.Linq;

namespace Market
{
    public class MarketSell : MonoBehaviour
    {
        [SerializeField] private Money moneyPrefab;
        [SerializeField] private TextMeshProUGUI moneyLabel;
        [SerializeField] private Interactable interactable;
        [SerializeField] private Transform moneySpawnPoint;
        [SerializeField] private TextMeshProUGUI sellText;
        [SerializeField] private Color defaultSellColor;
        [SerializeField] private Color highlihtedSellColor;

        private readonly HashSet<Sellable> sellables = new();
        private readonly HashSet<Money> money = new();

        private void OnEnable()
        {
            interactable.OnInteractionChanged += OnInteractionChanged;
            interactable.OnSelectionChanged += OnSelectionChanged;
            UpdateView();
        }
        private void OnDisable()
        {
            interactable.OnInteractionChanged -= OnInteractionChanged;
            interactable.OnSelectionChanged -= OnSelectionChanged;

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Sellable sellable))
            {
                sellables.Add(sellable);
                UpdateView();
            }
            if (other.gameObject.TryGetComponent(out Money money))
            {
                this.money.Add(money);
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.TryGetComponent(out Sellable sellable))
            {
                sellables.Remove(sellable);
                UpdateView();
            }
            if (other.gameObject.TryGetComponent(out Money money))
            {
                this.money.Remove(money);
            }
        }

        private void OnInteractionChanged(bool active)
        {
            if (active)
                Sell();
        }

        private void OnSelectionChanged(bool active)
        {
            sellText.color = active ? highlihtedSellColor : defaultSellColor;
        }

        private void Sell()
        {
            int reward = SumOfMoney();

            foreach (Sellable sellable in sellables)
            {
                Destroy(sellable.gameObject);
            }

            sellables.Clear();

            UpdateView();

            if (this.money.Count > 0)
            {
                Money money = this.money.Single();
                money.SetValue(money.Value + reward);
            }
            else
            {
                Money money = Instantiate(moneyPrefab);
                money.transform.position = moneySpawnPoint.position;
                money.SetValue(reward);
            }
        }

        private void UpdateView()
        {
            moneyLabel.text = $"{SumOfMoney()}$";
        }

        private int SumOfMoney()
        {
            int sum = 0;
            foreach (var item in sellables)
                sum += item.Price;
            return sum;
        }
    }
}