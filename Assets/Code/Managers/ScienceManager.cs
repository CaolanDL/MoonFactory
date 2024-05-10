using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScienceManager
{
    [NonSerialized] public List<StructureData> unlocked_Structures = new(); 
    [NonSerialized] public List<ResourceData> unlocked_Resources = new();

    public int SciencePoints = 0;

    public Action<int> PointsChanged;
    public Action ResearchCompleted;

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

    public void SetupNewGame()
    {
        var structures = GlobalData.Instance.Structures;

        foreach (var str in structures)
        {
            str.unlocked = false;  
        }
        foreach(var r in GlobalData.Instance.Resources)
        {
            r.unlocked = false;
        }
        GameManager.Instance.ScienceManager.unlocked_Structures.Clear();
        GameManager.Instance.ScienceManager.unlocked_Resources.Clear();

        foreach (var str in GlobalData.Instance.UnlockedOnStart)
        {
            str.Unlock();
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

        PointsChanged?.Invoke(SciencePoints);
        ResearchCompleted?.Invoke();
    }

    public void RemovePoints(int points)
    {
        SciencePoints -= points;
        PointsChanged?.Invoke(SciencePoints);
    }
}