using Interactables;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Interactable))]
public class SeedBag : Liftable
{
    [SerializeField] private SeedSO seedType;
    [SerializeField] private float fadeSpeed = 0.05f;

    private Interactable interactable;

    protected override void Awake()
    {
        base.Awake();
        interactable = GetComponent<Interactable>();
    }

    public override void Drop()
    {
        base.Drop();
        TryPlant();
    }

    private bool TryPlant() // zrobiæ tak by po ustawieniu FieldPatch,  FieldPatch zespawnowa³ planta (nowa klasa itd), a worek zosta³ w rêce
    {
        var target = LiftableHolder.SelectedObject;
        if (target != null && target.TryGetComponent(out FieldPatch patch))
        {
            StartCoroutine(Plant(patch));
            return true;
        }
        return false;
    }

    private IEnumerator Plant(FieldPatch patch)
    {
        patch.SetPlant(this);
        interactable.enabled = false;
        myRigidbody.isKinematic = true;
        myRigidbody.useGravity = false;
        var startPos = transform.position;
        var percent = 0f;
        while (percent < 1f)
        {
            transform.position = Vector3.Lerp(startPos, patch.PlantPoint.position, percent);
            percent += Time.deltaTime;
            yield return null;
        }
        transform.position = patch.PlantPoint.position;

        StartCoroutine(Fade(patch));
    }

    private IEnumerator Fade(FieldPatch patch)
    {
        while (transform.localScale.x > 0)
        {
            transform.localScale -= fadeSpeed * Vector3.one;
            yield return null;
        }
        patch.SetPlant(null);
        Destroy(gameObject);
    }
}
