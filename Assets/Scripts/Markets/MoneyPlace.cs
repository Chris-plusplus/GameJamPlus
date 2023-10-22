using Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoneyPlace : MonoBehaviour
{
    [SerializeField] private BoxCollider moneyArea;
    [SerializeField] private BuyingStation buyingStation;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Money money))
        {
            buyingStation.money = money;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Money money))
        {
            buyingStation.money = null;
        }
    }
}
