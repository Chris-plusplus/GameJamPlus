using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Plant : MonoBehaviour
{
    public FieldPatch fieldPatch;
    [SerializeField] private SeedSO seed;
    [SerializeField] private float time = 0;
    [SerializeField] private int nextStage = 0;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Init(FieldPatch f)
    {
        fieldPatch = f;
        transform.position = fieldPatch.PlantPoint.position;
        time = 0;
        nextStage = 0;
        StartCoroutine(Grow());
    }
    
    private IEnumerator Grow()
    {
        while(nextStage != seed.GrowStages.Count)
        {
            time += Time.deltaTime;
            if(time >= seed.GrowStages.ElementAt(nextStage).time)
            {
                float scaleX = seed.GrowStages.ElementAt(nextStage).scale / transform.parent.lossyScale.x;
                float scaleY = seed.GrowStages.ElementAt(nextStage).scale / transform.parent.lossyScale.y;
                float scaleZ = seed.GrowStages.ElementAt(nextStage).scale / transform.parent.lossyScale.z;
                transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
                ++nextStage;
            }
            yield return null;
        }
    }
}
