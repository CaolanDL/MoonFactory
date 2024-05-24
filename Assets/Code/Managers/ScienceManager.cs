using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScienceManager
{
    [NonSerialized] public List<StructureData> unlocked_structures = new(); 
    [NonSerialized] public List<ResourceData> unlocked_resources = new();

    public int SciencePoints = 0;

    public static Action<int> SciencePointsChanged;
    public static Action<ResourceData, int> ResearchCompleted;
    public static Action<ResourceData> OnResourceUnlocked;
    public static Action<StructureData> OnStructureUnlocked;

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
 
        GameManager.Instance.ScienceManager.unlocked_structures.Clear();
        GameManager.Instance.ScienceManager.unlocked_resources.Clear();

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
        var pointsToAdd = resource.ResearchValue * ResearchMultipliers[researcher];
        SciencePoints += pointsToAdd; 

        SciencePointsChanged?.Invoke(SciencePoints);
        ResearchCompleted?.Invoke(resource, pointsToAdd);

        if (TutorialProxy.IsActive)
        {
            TutorialProxy.Action?.Invoke(TutorialEvent.ResearchComplete);
        }
    }

    public void RemovePoints(int points)
    {
        SciencePoints -= points;
        SciencePointsChanged?.Invoke(SciencePoints);
    }

    public void TryUnlockStructure(StructureData structure)
    {
        if (unlocked_structures.Contains(structure)) return; 
        unlocked_structures.Add(structure);
        OnStructureUnlocked?.Invoke(structure);
    }

    public void TryUnlockResource(ResourceData resource)
    {
        if (unlocked_resources.Contains(resource)) return;
        unlocked_resources.Add(resource);
        OnResourceUnlocked?.Invoke(resource);
    }
}