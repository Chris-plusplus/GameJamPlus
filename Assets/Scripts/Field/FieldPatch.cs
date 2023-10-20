using Interactables;
using NaughtyAttributes;
using Newtonsoft.Json.Bson;
using QuickOutline;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class FieldPatch : Interactable
{
    [SerializeField]
    public bool occupied = false;
    [SerializeField]
    public SeedBag.SeedType type = SeedBag.SeedType.none;
    [SerializeField]
    public bool measureTime = false;
    [SerializeField]
    public float time = 0.0f;
    [SerializeField]
    public float nextTimeStamp = 0.0f;
    [SerializeField]
    public int stage = 0;
    [SerializeField]
    public int stageIndex = 0;

    private PlayerInteractions playerInteractions;
    private Dictionary<SeedBag.SeedType, List<(float, int)>> stages = new Dictionary<SeedBag.SeedType, List<(float, int)>>();
    private Outline outline;
    private InteractionHighlightController highlightController;
    
    void Start()
    {
        playerInteractions = FindObjectOfType<PlayerInteractions>();
        InitDictionary();
        OnSelectionChanged += FieldSelection;
        OnInteractionChanged += Harvest;
        outline = GetComponent<Outline>();
        outline.enabled = false;
        highlightController = GetComponent<InteractionHighlightController>();
        highlightController.enabled = false;
        occupied = false;
    }
    
    public void FieldSelection(bool isInteracting)
    {
        if(isInteracting && !occupied)
        {
            Liftable bagObj = playerInteractions.HeldObject;
            LiftableSeedBag bag = null;
            if (bagObj != null)
            {
                bag = bagObj.GetComponent<LiftableSeedBag>();
            }
            if (bag != null)
            {
                outline.enabled = true;
            }
        }
        else
        {
            outline.enabled = false;
        }
    }

    void Update()
    {
        UpdateTime();
    }

    public void Harvest(bool clicked)
    {
        if (clicked && highlightController.enabled)
        {
            Debug.Log("harvested");
            Destroy();
        }
    }
    [Button("Destroy", EButtonEnableMode.Always)]
    public void Destroy()
    {
        this.type = SeedBag.SeedType.none;
        time = 0.0f;
        stageIndex = 0;
        nextTimeStamp = 0.0f;
        stage = 0;
        measureTime = false;
        highlightController.enabled = false;
        outline.enabled = false;
        occupied = false;
    }

    public void Set(SeedBag.SeedType type)
    {
        Destroy();
        occupied = true;
        this.type = type;
        nextTimeStamp = stages[type][stageIndex].Item1;
        stage = stages[type][stageIndex].Item2;
        measureTime = true;
    }
    private void UpdateTime()
    {
        if (measureTime)
        {
            time += Time.deltaTime;
            if (time >= nextTimeStamp)
            {
                if (stageIndex < stages[type].Count)
                {
                    stage = stages[type][stageIndex].Item2;
                }
                ++stageIndex;
                if (stageIndex < stages[type].Count)
                {
                    nextTimeStamp = stages[type][stageIndex].Item1;
                }
                else
                {
                    measureTime = false;
                    time = nextTimeStamp;
                    highlightController.enabled = true;
                    if(playerInteractions.SelectedObject == this)
                    {
                        Select();
                    }
                }
            }
        }
    }
    private void InitDictionary()
    {
        foreach (SeedBag.SeedType val in Enum.GetValues(typeof(SeedBag.SeedType)))
        {
            stages.Add(val, new List<(float, int)>());
        }
        stages[SeedBag.SeedType.placeholder].Add((0.0f, 0));
        stages[SeedBag.SeedType.placeholder].Add((2.0f, 1));
        stages[SeedBag.SeedType.placeholder].Add((4.0f, 2));
        stages[SeedBag.SeedType.placeholder].Add((6.0f, 3));
    }
}
