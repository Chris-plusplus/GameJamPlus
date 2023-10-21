using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Seed")]
public class SeedSO : ScriptableObject
{
    [System.Serializable]
    public struct GrowStage
    {
        public float time;
    }

    [SerializeField] private GrowStage[] growStages;

    public IReadOnlyCollection<GrowStage> GrowStages => growStages;
}
