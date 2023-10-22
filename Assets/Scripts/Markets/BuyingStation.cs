using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using Interactables;

public class BuyingStation : MonoBehaviour
{
    [SerializeField] public SphereCollider marketArea;
    [SerializeField] public BoxCollider moneyArea;
    [SerializeField] private List<Buyable> buyables;
    [SerializeField] public Money money;

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
    }

    public void TryBuy(Buyable buyable)
    {
        if(money != null)
        {
            int index = buyables.FindIndex(a => a == buyable);
            var newBuyable = Instantiate(buyableCopies[index]);
            newBuyable.gameObject.SetActive(true);
            buyables[index] = newBuyable;
            newBuyable.ResetPos();
            money.RemoveValue();

            buyable.enabled = false;
        }
        else
        {
            buyable.AntiTheft();
        }
    }
}
