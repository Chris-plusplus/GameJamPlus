using TMPro;
using UnityEngine;

namespace Market
{
    public class MoneyTextUpdater : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moneyLabel;
        [SerializeField] private MoneyPlace moneyPlace;

        private void OnEnable()
        {
            moneyPlace.OnMoneyChange += UpdateView;
            UpdateView();
        }
        private void OnDisable()
        {
            moneyPlace.OnMoneyChange -= UpdateView;
        }

        private void UpdateView()
        {
            moneyLabel.text = $"{moneyPlace.GetSum()}$";
        }
    }
}