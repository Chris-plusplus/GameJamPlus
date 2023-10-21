using Interactables;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class SeedBag : Liftable
{
    [SerializeField] private SeedSO seedType;
    //[SerializeField] private float fadeSpeed = 0.05f;
    [SerializeField] private int seedCount;

    private Interactable interactable;

    protected override void Awake()
    {
        base.Awake();
        interactable = GetComponent<Interactable>();
    }

    public override void Drop()
    {
        if (!TryPlant())
        {
            base.Drop();
        }
        else if(seedCount <= 0)
        {
            base.Drop();
            Destroy(gameObject);
        }
    }

    private bool TryPlant() // zrobiæ tak by po ustawieniu FieldPatch,  FieldPatch zespawnowa³ planta (nowa klasa itd), a worek zosta³ w rêce
    {
        var target = LiftableHolder.SelectedObject;
        if (target != null && target.TryGetComponent(out FieldPatch fieldPatch))
        {
            Seed seed = Instantiate(seedType.seed, transform.position, transform.rotation);
            seed.PlantAt(fieldPatch);
            --seedCount;
            return true;
        }
        return false;
    }

    /*
    private IEnumerator Plant(FieldPatch fieldPatch)
    {
        fieldPatch.isOccupied = true;
        interactable.enabled = false;
        myRigidbody.isKinematic = true;
        myRigidbody.useGravity = false;
        var startPos = transform.position;
        var percent = 0f;
        while (percent < 1f)
        {
            transform.position = Vector3.Lerp(startPos, patch.SeedPoint.position, percent);
            percent += Time.deltaTime;
            yield return null;
        }
        transform.position = fieldPatch.SeedPoint.position;
        //patch.SetPlant(seedType.prefab);
        StartCoroutine(Fade(fieldPatch));
    }

    private IEnumerator Fade(FieldPatch fieldPatch)
    {
        while (transform.localScale.x > 0)
        {
            transform.localScale -= fadeSpeed * Vector3.one;
            yield return null;
        }
        fieldPatch.SetPlant(seedType.prefab);
        Destroy(gameObject);
    }
    */
}
