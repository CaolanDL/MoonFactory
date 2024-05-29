using ExtensionMethods;
using MoonFactory.Interfaces;
using RoverTasks; 
using UnityEngine;

namespace Terrain
{
    public class Crater : Entity
    { 
        public CraterData craterData;

        public Crater(CraterData craterData)
        {
            this.craterData = craterData;
            size = new(craterData.size, craterData.size);
        }
    }
}