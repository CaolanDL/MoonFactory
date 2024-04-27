using Logistics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleAnalyserInterface : StaticInterface, IResearchInterface
{
    [SerializeField] Image sampleSprite;

    public IDoResearch researcher { get; set; }

    SampleAnalyser sampleAnalyser;
    [SerializeField] ProgressBar researchProgress;

    public override void Init(Entity entity, Vector3 screenPosition)
    {
        base.Init(entity, screenPosition);
         
        sampleAnalyser = (SampleAnalyser)entity;
        researcher = sampleAnalyser;
    }

    private void Update()
    {
        UpdateUI();
    } 

    public override void UpdateUI()
    {
        base.UpdateUI();

        researchProgress.SetProgress(sampleAnalyser.progress);

        //Debug.Log(sampleAnalyser);

        if(sampleAnalyser.sample != null)
        {
            sampleSprite.enabled = true;
            sampleSprite.sprite = sampleAnalyser.sample.sprite;
        }
        else
        {
            sampleSprite.enabled = false;
        }
    } 

    public IRequestResources GetIRequestResources()
    { 
        return (IRequestResources)sampleAnalyser;
    }
}

public interface IResearchInterface
{
    public IRequestResources GetIRequestResources();

    public IDoResearch researcher { get; set; }
}