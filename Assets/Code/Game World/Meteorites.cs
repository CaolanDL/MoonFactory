using System.Collections;
using UnityEngine;

namespace Meteorites
{
    public class Meteorite : Entity
    {
        public ModelData modelData;

        public float scale;

        public Meteorite(ModelData modelData, float scale) : base()
        {
            this.modelData = modelData;
            this.scale = scale;
        }
    }
}