using System; 
using System.Collections.Generic;
using UnityEngine;
 
public class DisplayObject : MonoBehaviour
{
    [NonSerialized] public Entity parentEntity;
     
    public List<GameObject> primaryModels;
    [NonSerialized] public GameObject activeModel;
    public List<GameObject> additiveModels; 
    public Dictionary<string, GameObject> particleSystems = new(); 
    public Animator animator;
    public Transform wireConnectionPoint;
         
    private string activeAnimation;


    private void Awake()
    {
        if (primaryModels.Count == 0)
        {
            primaryModels.Add(GetComponentInChildren<MeshRenderer>().gameObject);
        }
        if(primaryModels[0] == null)
        {
            primaryModels[0] = GetComponentInChildren<MeshRenderer>().gameObject;
        }

        foreach (var model in primaryModels)
        {
            model.SetActive(false); 
        }

        if (primaryModels.Count == 0) { return; }

        primaryModels[0].SetActive(true);

        activeModel = primaryModels[0];

        animator = GetComponentInChildren<Animator>(); 
         
        var particleSystemComponents = GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem component in particleSystemComponents)
        {  
            particleSystems.Add(component.gameObject.name, component.gameObject);

            component.gameObject.SetActive(false);
        }
    }

    public void DemolishAnimation()
    {
        // play demolish animation

        // Destroy self once complete
        //todo migrate to a different method
        if(gameObject != null)
        {
            Destroy(gameObject); 
        }
    }

    public void PlayAnimationOnce(string animationName)
    {

    }

    public void SetLoopingAnimation(string animationName)
    {
        if(animator == null) { return; }

        animator.Play(animationName); 
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

    public void PlayParticleEffect(string name)
    {
        if (particleSystems.ContainsKey(name) == false) { return; }
        particleSystems[name].SetActive(true);
    }

    public void StopParticleEffect(string name)
    {
        if(particleSystems.ContainsKey(name) == false) { return; }
        particleSystems[name].SetActive(false);
    }

    public void PlaySound()
    {

    }

    public Vector3 GetWireConnectionPoint()
    {
        if(wireConnectionPoint != null)
        {
            return wireConnectionPoint.position;
        }

        return transform.position + (Vector3.up * 0.5f);
    }
} 