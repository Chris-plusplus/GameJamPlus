using Interactables;
using QuickOutline;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class SeedBag : MonoBehaviour
{
    public enum SeedType
    {
        none,
        placeholder
    }

    [SerializeField]
    public SeedType type = SeedType.none;

    private Rigidbody myRigidbody;
    private Collider myCollider;
    private Transform myTransform;
    private LiftableSeedBag liftableSeedBag;
    public InteractionHighlightController interactionHighlightController;
    [SerializeField]
    public float fadeSpeed = 0.05f;
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myCollider = GetComponent<Collider>();
        myTransform = GetComponent<Transform>();
        liftableSeedBag = GetComponent<LiftableSeedBag>();
        interactionHighlightController = GetComponent<InteractionHighlightController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FadeBegin()
    {
        gameObject.layer = 10;
        //GetComponent<Outline>().enabled = false;
        Destroy(interactionHighlightController);
        Destroy(liftableSeedBag);
        Destroy(myRigidbody);
        myCollider.enabled = false;
    }
    private IEnumerator Fade(FieldPatch patch)
    {
        FadeBegin();
        for (; ; )
        {
            myTransform.localScale = myTransform.localScale - fadeSpeed*Vector3.one;// new Vector3(transform.localScale.x - fadeSpeed, transform.localScale.y - fadeSpeed, transform.localScale.z - fadeSpeed);
            if(myTransform.localScale.x <= 0)
            {
                patch.Set(this.type);
                Destroy(this.gameObject);
            }
            yield return null;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        FieldPatch patch = collision.gameObject.GetComponent<FieldPatch>();
        if(patch != null && !patch.occupied)
        {
            patch.occupied = true;
            StartCoroutine(Fade(patch));
        }
    }
}
