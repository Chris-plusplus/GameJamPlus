using Interactables;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Plant : Interactable
{
    public FieldPatch fieldPatch;
    [SerializeField] private SeedSO seed;
    [SerializeField] private float time = 0;
    [SerializeField] private int nextStage = 0;
    [SerializeField] public bool Grown => nextStage == seed.GrowStages.Count;

    private InteractionHighlightController outlineController;

    protected override void Awake()
    {
        base.Awake();
        outlineController = GetComponent<InteractionHighlightController>();
        outlineController.enabled = false;
        OnSelectionChanged += UpdateSelect;
    }

    public void Init(FieldPatch f)
    {
        fieldPatch = f;
        transform.position = fieldPatch.PlantPoint.position;
        time = 0;
        nextStage = 0;
        StartCoroutine(Grow());
    }

    private void Harvest(bool input)
    {
        if (input)
        {
            fieldPatch.Destroy();
            Destroy(gameObject);
            // dodaæ do inventory czy coœ
        }
    }
    
    private IEnumerator Grow()
    {
        while(nextStage != seed.GrowStages.Count)
        {
            time += Time.deltaTime;
            if(time >= seed.GrowStages.ElementAt(nextStage).time)
            {
                transform.localScale = seed.GrowStages.ElementAt(nextStage).scale * Vector3.one;
                ++nextStage;
            }
            yield return null;
        }
        UpdateSelect(Interacter?.SelectedObject == this);
    }
    private void UpdateSelect(bool isSelected)
    {
        if (isSelected && Grown && Interacter is ILiftableHolder liftableHolder && liftableHolder.HeldObject is null)
        {
            outlineController.UpdateOutline(true);
            OnInteractionChanged += Harvest;
            ShowPointerOnInterract = true;
        }
        else
        {
            outlineController.UpdateOutline(false);
            OnInteractionChanged -= Harvest;
            ShowPointerOnInterract = false;
        }
    }
}
