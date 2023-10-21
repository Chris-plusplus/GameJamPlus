using Interactables;
using UnityEngine;

public class FieldPatch : Interactable
{
    [SerializeField] private Transform seedPoint;
    [SerializeField] private Transform plantPoint;

    private Plant plant;

    public Transform SeedPoint => seedPoint;
    public Transform PlantPoint => plantPoint;
    public bool isOccupied = false;

    public void SetPlant(Plant plant)
    {
        this.plant = Instantiate(plant, null);
        this.plant.Init(this);
    }
}
