using Interactables;
using QuickOutline;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LiftableSeedBag : Liftable
{
    private Rigidbody myRigidbody;
    private Collider myCollider;
    private PlayerInteractions playerInteractions;
    private SeedBag bag;
    private float radius;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        playerInteractions = FindObjectOfType<PlayerInteractions>();
        bag = FindObjectOfType<SeedBag>();
        radius = GetComponent<SphereCollider>().radius;
    }
    public override void PickUp(int layer)
    {
        base.PickUp(layer);
        myCollider.enabled = false;
    }
    public override void Drop()
    {
        Interactable selected = playerInteractions.SelectedObject;
        FieldPatch field = null;
        if(selected != null)
        {
            field = selected.GetComponent<FieldPatch>();
        }
        if (field != null && !field.occupied)
        {
            Destroy(GetComponent<InteractionHighlightController>());
            Destroy(GetComponent<Outline>());
            transform.position = field.transform.position + new Vector3(0, radius);
            field.Destroy();
        }
        base.Drop();
        //GetComponent<Outline>().enabled = false;
        myCollider.enabled = true;
    }
}
