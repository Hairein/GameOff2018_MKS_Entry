using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GO2018_MKS_MessageLibrary.MessageLibraryUtitlity;

namespace GO2018_MKS_MessageLibrary
{

    public class BarricadeResourceState
    {
        public string Name = string.Empty;

        public WorldCoordinate Position = new WorldCoordinate();

        public float ResourceCount = 0.0f;

        public BarricadeResourceState()
        {
        }

        public BarricadeResourceState(string newName, WorldCoordinate newPosition, float newResourceCount)
        {
            Name = newName;

            Position = newPosition;

            ResourceCount = newResourceCount;
        }
    }
}
