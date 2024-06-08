using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class LanderSequence : MonoBehaviour
{
    [SerializeField] float initialDelay = 2.0f;
    float delayTime = 1f;
    bool hasStartedAnimation = false;

    [SerializeField] List<ParticleSystem> particles = new();

    Animator Animator;

    private void Awake()
    {
        GameManager.OnGameExit += DestroySelf;

        Animator = GetComponent<Animator>();
    }

    private void OnDestroy() => GameManager.OnGameExit -= DestroySelf;
    private void OnDisable() => GameManager.OnGameExit -= DestroySelf;

    void DestroySelf() => Destroy(transform.parent.gameObject);

    private void Update()
    {
        if(delayTime > 0f) { delayTime -= Time.deltaTime / initialDelay; }
        if(delayTime < 0f && !hasStartedAnimation) { Animator.Play("LandingSequence"); hasStartedAnimation = true; }
    }

    public void Landed()
    {
        foreach(var particle in particles)
        {
            particle.Stop();
        }
    }

    public void Complete()
    {
        var widget = RoverManager.Instance.SpawnNewWidget(new int2(0, -1));
        widget.GridRotation = 2;
        widget.VisualRotation = 180f;
        widget.DisplayObject.transform.rotation = Quaternion.Euler(0, 180, 0); 
        GameManager.Instance.ConstructionManager.ForceSpawnStructure(new int2(0, 0), 0, GlobalData.Instance.Structures.Find(x => x.name == "Lander"));
        Destroy(gameObject);

        TutorialProxy.Action?.Invoke(TutorialEvent.BeginTutorial);

        GameManager.Instance.AudioManager.StartMusic();
    }
}
