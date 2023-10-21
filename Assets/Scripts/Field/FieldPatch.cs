using Interactables;
using UnityEngine;

public class FieldPatch : Interactable
{
    [SerializeField] private Transform seedPoint;
    [SerializeField] private Transform plantPoint;

    private Plant plant;
    private InteractionHighlightController outlineController;

    public Transform SeedPoint => seedPoint;
    public Transform PlantPoint => plantPoint;
    public bool isOccupied = false;

    protected override void Awake()
    {
        base.Awake();
        outlineController = GetComponent<InteractionHighlightController>();
        outlineController.enabled = false;
        OnSelectionChanged += UpdateSelect;
        UpdateSelect(false);
    }

    public void SetPlant(Plant plant)
    {
        this.plant = Instantiate(plant, null);
        this.plant.Init(this);
    }

    public void Destroy()
    {
        isOccupied = false;
    }

    private void UpdateSelect(bool isSelected)
    {
        if (isSelected && !isOccupied && Interacter is ILiftableHolder liftableHolder && liftableHolder.HeldObject is SeedBag)
        {
            outlineController.UpdateOutline(true);
            ShowPointerOnInterract = true;
        }
        else
        {
            outlineController.UpdateOutline(false);
            ShowPointerOnInterract = false;
        }
    }
    public void Occupy()
    {
        isOccupied = true;
        UpdateSelect(true);
    }
}
