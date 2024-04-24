using Logistics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SampleAnalyserInterface : StaticInterface, IResearchInterface
{
    SampleAnalyser sampleAnalyser;

    [SerializeField] ProgressBar researchProgress;

    public override void Init(Entity entity, Vector3 screenPosition)
    {
        base.Init(entity, screenPosition);

        sampleAnalyser = (SampleAnalyser)entity;
    }

    private void Update()
    {
        UpdateUI();
    }

    public override void UpdateUI()
    {
        base.UpdateUI();

        researchProgress.SetProgress(Mathf.PingPong(Time.fixedTime, 1));
    }

    public IRequestResources GetIRequestResources()
    {
        return (IRequestResources)sampleAnalyser;
    }
}

public interface IResearchInterface
{
    public IRequestResources GetIRequestResources();
}