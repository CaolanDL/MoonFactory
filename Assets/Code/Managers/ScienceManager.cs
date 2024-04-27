using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScienceManager
{
    public int SciencePoints = 0;

    public Action<int> PointsAdded;

    public enum Researcher
    {
        Analyser,
        Rocket,
        Railgun
    }

    public Dictionary<Researcher, Dictionary<ResourceData, bool>> ResearchRegistries = new();

    Dictionary<Researcher, int> ResearchMultipliers = new()
    {
        { Researcher.Analyser, 10 },
        { Researcher.Rocket, 40 },
        { Researcher.Railgun, 160 },
    };

    public ScienceManager()
    {
        ResearchRegistries.Add(Researcher.Analyser, new());
        ResearchRegistries.Add(Researcher.Rocket, new());
        ResearchRegistries.Add(Researcher.Railgun, new());

        // Populate Registries
        foreach (var resource in GlobalData.Instance.Resources) 
            foreach (var registryKVP in ResearchRegistries)
            {
                registryKVP.Value.Add(resource, false);
            } 
    }

    public bool IsResearchedIn(ResourceData resource, Researcher researcher)
    {
        return ResearchRegistries[researcher][resource];
    } 



    public void ResearchComplete(ResourceData resource, Researcher researcher)
    {
        ResearchRegistries[researcher][resource] = true;
        SciencePoints += resource.ResearchValue * ResearchMultipliers[researcher]; 

        PointsAdded?.Invoke(SciencePoints);
    }
}