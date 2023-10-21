using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Seed : MonoBehaviour
{
    [SerializeField] private float throwForceMultiplier = 15f;
    [SerializeField] private float maxLifeTime = 4f;

    private SeedSO seedType;
    private FieldPatch fieldPatch;
    private Rigidbody rb;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent(out FieldPatch fieldPatch) == this.fieldPatch)
            OnHitField();
    }

    public void Setup(SeedSO seedType, FieldPatch fieldPatch)
    {
        this.seedType = seedType;
        fieldPatch.SetOccupy(true);

        this.fieldPatch = fieldPatch;
        rb = GetComponent<Rigidbody>();

        var targetPosition = fieldPatch.transform.position;
        Vector3 toTarget = targetPosition - transform.position;
        float gSquared = Physics.gravity.sqrMagnitude;
        float b = throwForceMultiplier * throwForceMultiplier + Vector3.Dot(toTarget, Physics.gravity);
        float throwForce = Mathf.Sqrt(b * 2f / gSquared);
        Vector3 velocity = toTarget / throwForce - Physics.gravity * throwForce / 2f;
        rb.AddForce(velocity, ForceMode.VelocityChange);

        StartCoroutine(WaitToDispawn());
    }

    private void OnHitField()
    {
        StopAllCoroutines();
        fieldPatch.SetPlant(seedType.prefab);
        Destroy(gameObject);
    }

    private IEnumerator WaitToDispawn()
    {
        yield return new WaitForSeconds(maxLifeTime);
        OnFieldMiss();
    }
    private void OnFieldMiss()
    {
        StopAllCoroutines();
        fieldPatch.SetOccupy(false);
        Destroy(gameObject);
    }
}
