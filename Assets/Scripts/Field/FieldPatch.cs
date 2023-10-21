using Interactables;
using UnityEngine;

public class FieldPatch : Interactable
{
    [SerializeField] private Transform seedPoint;
    [SerializeField] private Transform plantPoint;

    private Growable plant;
    private InteractionHighlightController outlineController;
    private bool isOccupied = false;

    public Transform SeedPoint => seedPoint;
    public Transform PlantPoint => plantPoint;
    public bool IsOccupied => isOccupied;

    protected override void Awake()
    {
        base.Awake();
        outlineController = GetComponent<InteractionHighlightController>();
        outlineController.enabled = false;
        OnSelectionChanged += UpdateSelect;
        UpdateSelect(false);
    }

    public void SetPlant(Growable plant)
    {
        this.plant = Instantiate(plant, null);
        this.plant.Init(this);
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
    public void SetOccupy(bool isOccupied)
    {
        this.isOccupied = isOccupied;
        UpdateSelect(isOccupied);
    }
}
