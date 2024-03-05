using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
public class DisplayObject : MonoBehaviour
{
    public Entity parentEntity;

    public List<GameObject> models;

    public GameObject activeModel;

    private void Awake()
    {
        foreach (var model in models)
        {
            model.SetActive(false); 
        }

        models[0].SetActive(true);

        activeModel = models[0];
    }

    public void ChangeAnimation(string animationName)
    {

    }

    public void SetActiveModel(string modelName)
    {
        foreach (var model in models)
        {
            model.SetActive(false);
        }

        foreach (var model in models)
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