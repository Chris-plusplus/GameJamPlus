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
                transform.localScale = seed.GrowStages.ElementAt(nextStage).scale * Vector3.one;
                ++nextStage;
            }
            yield return null;
        }
    }
}
