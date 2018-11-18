using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public class WorldCoordinate
    {
        public float X = 0.0f;
        public float Y = 0.0f;
        public float Z = 0.0f;

        public WorldCoordinate()
        {

        }

        public WorldCoordinate(float newX, float newY, float newZ)
        {
            X = newX;
            Y = newY;
            Z = newZ;
        }
    }
}
