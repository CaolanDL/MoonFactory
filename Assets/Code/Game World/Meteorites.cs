using ExtensionMethods;
using MoonFactory.Interfaces;
using RoverTasks; 
using UnityEngine;

namespace Terrain
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
            GameManager.Instance.GameWorld.meteorites.Remove(position);
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

        public void TryUpdate()
        {
            if (flaggedForDemolition && demolishIcon == null)
            {
                demolishIcon = GameObject.Instantiate(MenuData.Instance.DemolishSprite, GameManager.Instance.HUDManager.WorldIconContainer);
            }
            if (demolishIcon != null)
            {
                demolishIcon.transform.position = position.ToVector3().ToScreenPosition();
                demolishIcon.transform.localScale = Vector3.one / GameManager.Instance.CameraController.zoom;
                if (!flaggedForDemolition)
                {
                    GameObject.Destroy(demolishIcon);
                }
            }
        }
    }
}