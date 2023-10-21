using Interactables;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seed : MonoBehaviour
{
    [SerializeField] private float moveTime;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private SeedSO seedType;

    public void PlantAt(FieldPatch fieldPatch)
    {
        fieldPatch.Occupy();
        StartCoroutine(MoveTo(fieldPatch));
    }
    private IEnumerator MoveTo(FieldPatch fieldPatch)
    {
        var startPos = transform.position;
        var percent = 0f;
        while (percent < 1f)
        {
            transform.position = Vector3.Lerp(startPos, fieldPatch.SeedPoint.position, percent);
            percent += Time.deltaTime / moveTime;
            yield return null;
        }
        transform.position = fieldPatch.SeedPoint.position;
        StartCoroutine(Fade(fieldPatch));
    }
    private IEnumerator Fade(FieldPatch fieldPatch)
    {
        float scale = 1;
        float startScale = transform.localScale.x;
        while(scale > 0)
        {
            transform.localScale = startScale * scale * Vector3.one;
            scale -= fadeSpeed;
            yield return null;
        }
        fieldPatch.SetPlant(seedType.prefab);
        Destroy(gameObject);
    }
}
