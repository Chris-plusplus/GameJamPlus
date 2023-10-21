using Interactables;
using UnityEngine;

public class FieldPatch : Interactable
{
    [SerializeField] private Transform plantPoint;

    private SeedBag plant; // trzeba to przerobiæ by by³a klasa plant któr¹ siê sadzie


    public Transform PlantPoint => plantPoint;
    public bool IsOccupied => plant != null;


    public void SetPlant(SeedBag plant)
    {
        this.plant = plant;
    }
}
