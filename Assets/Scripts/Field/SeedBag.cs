using Interactables;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class SeedBag : Liftable
{
    [SerializeField] private SeedSO seedType;
    [SerializeField] private int seedCount;
    [SerializeField] private Transform seedPoint;

    private Interactable interactable;
    private IInteracter interacter;

    protected override void Awake()
    {
        base.Awake();
        interactable = GetComponent<Interactable>();
    }

    protected override void OnInteractionChanged(bool isInteracting)
    {
        if (isInteracting)
        {
            if (!TryPlant())
            {
                Holder.DropObject(this);
            }
            else if (seedCount <= 0)
            {
                Holder.DropObject(this);
                Destroy(gameObject);
            }
        }
    }
    public override void PickUp(ILiftableHolder holder)
    {
        base.PickUp(holder);
        interacter = interactable.Interacter;
    }
    private bool TryPlant()
    {
        var target = interacter.SelectedObject;
        if (target != null && target.TryGetComponent(out FieldPatch fieldPatch) && !fieldPatch.IsOccupied)
        {
            Seed seed = Instantiate(seedType.seed, seedPoint.position, transform.rotation);
            seed.Setup(seedType, fieldPatch);
            seedCount--;
            return true;
        }
        return false;
    }
}
