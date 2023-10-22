using Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buyable : MonoBehaviour
{
    [SerializeField] private int price;
    [SerializeField] private BuyingStation buyingStation;
    private Liftable liftable;
    private Rigidbody myRigidbody;
    public Interactable interactable;

    public int Price => price;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        interactable.OnInteractionChanged += TryBuy;
        liftable = GetComponent<Liftable>();
        myRigidbody = GetComponent<Rigidbody>();
        ResetPos();
    }

    public void Buy()
    {
        interactable.OnInteractionChanged -= TryBuy;
    }

    public void TryBuy(bool clicked)
    {
        if (clicked)
        {
            buyingStation.TryBuy(this);
        }
    }

    public void InteractableSetActive(bool active)
    {
        if (interactable)
        {
            interactable.enabled = active;
        }
        else
        {
            interactable = GetComponent<Interactable>();
            interactable.enabled = active;
        }
    }

    /*
    private void OnTriggerExit(Collider other)
    {
        if(other == buyingStation.marketArea)
        {
            buyingStation.TryBuy(this);
        }
    }
    
    */
    public void ResetPos()
    {
        //myRigidbody.velocity = Vector3.zero;
        //transform.position = origin.position;
        //transform.rotation = origin.rotation;
    }
    public void AntiTheft()
    {
        liftable.Holder.DropObject(liftable);
        ResetPos();
    }
    
}
