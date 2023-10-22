using Interactables;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyingSign : Interactable
{
    private GameObject money;
    private InteractionHighlightController highlightController;

    protected override void Awake()
    {
        base.Awake();
        highlightController = GetComponent<InteractionHighlightController>();
        highlightController.enabled = false;
        ShowPointerOnInterract = false;
    }
    public void UpdateSign(Buyable buyable)
    {
        money = null;

        if(money != null)
        {
            OnInteractionChanged += TryBuy;
            highlightController.enabled = true;
            ShowPointerOnInterract = true;
            highlightController.UpdateOutline(IsSelected);
        }
        else
        {
            OnInteractionChanged -= TryBuy;
            highlightController.enabled = false;
            ShowPointerOnInterract = false;
            highlightController.UpdateOutline(false);
        }
    }

    private void TryBuy(bool clicked)
    {
        if (true) // money
        {

        }
    }
}
