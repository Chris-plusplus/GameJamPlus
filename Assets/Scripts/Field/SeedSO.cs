using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/Seed")]
public class SeedSO : ScriptableObject
{
    [System.Serializable]
    public struct GrowInfo
    {
        public float time;
        public float scale;
    }

    [SerializeField] public GrowInfo growInfo;
    [SerializeField] public Growable prefab;
    [SerializeField] public Seed seed;
}
