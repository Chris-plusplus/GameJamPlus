using Interactables;
using UnityEngine;

public class FieldPatch : Interactable
{
    [SerializeField] private Transform plantPoint;

    private SeedBag plant; // trzeba to przerobi� by by�a klasa plant kt�r� si� sadzie


    public Transform PlantPoint => plantPoint;
    public bool IsOccupied => plant != null;


    public void SetPlant(SeedBag plant)
    {
        this.plant = plant;
    }
}
