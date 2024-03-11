using System; 
using System.Collections.Generic;
using UnityEngine;
 
public class DisplayObject : MonoBehaviour
{
    [NonSerialized] public Entity parentEntity;

    public List<GameObject> primaryModels;

    [NonSerialized] public GameObject activeModel;

    public List<GameObject> additiveModels;

    private void Awake()
    {
        foreach (var model in primaryModels)
        {
            model.SetActive(false); 
        }

        if (primaryModels.Count == 0) { return; }

        primaryModels[0].SetActive(true);

        activeModel = primaryModels[0];
    }

    public void ChangeAnimation(string animationName)
    {

    }

    public void SetActiveModel(string modelName)
    {
        foreach (var model in primaryModels)
        {
            model.SetActive(false);
        }

        foreach (var model in primaryModels)
        {
            if(model.name == modelName)
            {
                model.SetActive(true);
            }
        }
    }

    public void PlayParticleEffect()
    {

    }

    public void PlaySound()
    {

    }
} 