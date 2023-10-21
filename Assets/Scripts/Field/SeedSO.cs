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
        public float scale;
    }

    [SerializeField] private GrowStage[] growStages;
    [SerializeField] public Plant prefab;
    [SerializeField] public Seed seed;

    public IReadOnlyCollection<GrowStage> GrowStages => growStages;
}
