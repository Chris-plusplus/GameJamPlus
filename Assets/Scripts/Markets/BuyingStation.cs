using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Interactables;

public class BuyingStation : MonoBehaviour
{
    [SerializeField] private List<Buyable> buyables;
    [SerializeField] public MoneyPlace moneyPlace;

    private List<Buyable> buyableCopies;

    protected void Awake()
    {
        buyableCopies = new();
        foreach(var buyable in buyables)
        {
            var newBuyable = Instantiate(buyable);
            buyableCopies.Add(newBuyable);
            newBuyable.gameObject.SetActive(false);
        }
        UpdateBuyables();
    }

    public void UpdateBuyables()
    {
        if(moneyPlace.GetSum() > 0)
        {
            foreach (var buyable in buyables)
            {
                buyable.InteractableSetActive(true);
            }
        }
        else
        {
            foreach (var buyable in buyables)
            {
                buyable.InteractableSetActive(false);
            }
        }
    }

    public void TryBuy(Buyable buyable)
    {
        if (buyable.Price >= moneyPlace.GetSum())
        {
            int index = buyables.FindIndex(a => a == buyable);
            var newBuyable = Instantiate(buyableCopies[index]);
            newBuyable.gameObject.SetActive(true);
            buyables[index] = newBuyable;
            newBuyable.ResetPos();

            moneyPlace.RemoveSum(buyable.Price);

            buyable.enabled = false;
            buyable.Buy();
        }
        else
        {
            //buyable.AntiTheft();
        }
    }
}
