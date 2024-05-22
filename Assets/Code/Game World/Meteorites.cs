using MoonFactory.Interfaces;
using RoverTasks;
using System.Collections;
using UnityEngine;

namespace Meteorites
{
    public class Meteorite : Entity, IDemolishable
    {
        public ModelData modelData; 
        public float scale;

        public Meteorite(ModelData modelData, float scale) : base()
        {
            this.modelData = modelData;
            this.scale = scale;
        }

        public ManagedTask demolishTask = new();
        public bool flaggedForDemolition;
        GameObject demolishIcon;

        public int DemolishTime => 50*5;

        public void Demolish()
        {
            GameObject.Destroy(demolishIcon); demolishIcon = null; 
            RemoveEntity();
            //Spawn Demolish Particles 
        }

        public void ToggleDemolition()
        {
            if(flaggedForDemolition) CancelDemolition(); 
            else FlagForDemolition(); 
        }

        public void FlagForDemolition()
        {
            flaggedForDemolition = true; 
            var demolishTask = new RoverTasks.DemolishTask(this); 
            this.demolishTask.TryCreateTask(demolishTask);
        }

        public void CancelDemolition()
        {
            demolishTask.CancelTask(); 
            flaggedForDemolition = false;
        } 
    }
}