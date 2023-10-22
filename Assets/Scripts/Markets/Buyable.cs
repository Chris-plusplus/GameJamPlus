using Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Buyable : MonoBehaviour
{
    [SerializeField] private int price;
    [SerializeField] private Transform origin;
    [SerializeField] private BuyingStation buyingStation;
    private Liftable liftable;
    private Rigidbody myRigidbody;
    private Interactable interactable;

    public int Price => price;

    private void Awake()
    {
        interactable = GetComponent<Interactable>();
        liftable = GetComponent<Liftable>();
        myRigidbody = GetComponent<Rigidbody>();
        ResetPos();
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
