using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class LanderSequence : MonoBehaviour
{
    [SerializeField] List<ParticleSystem> particles = new();

    public void Landed()
    {
        foreach(var particle in particles)
        {
            particle.Stop();
        }
    }

    public void Complete()
    {
        var widget = RoverManager.Instance.SpawnWidget(new int2(0, -1));
        widget.rotation = 2;
        widget.DisplayObject.transform.rotation = Quaternion.Euler(0, 180, 0); 
        GameManager.Instance.ConstructionManager.ForceSpawnStructure(new int2(0, 0), 0, GlobalData.Instance.Structures.Find(x => x.name == "Lander"));
        Destroy(gameObject);
    }
}
