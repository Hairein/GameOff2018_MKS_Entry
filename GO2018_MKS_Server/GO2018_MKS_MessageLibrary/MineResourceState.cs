using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GO2018_MKS_MessageLibrary.MessageLibraryUtitlity;

namespace GO2018_MKS_MessageLibrary
{

    public class MineResourceState
    {
        public string Name = string.Empty;

        public MineType MineType;

        public float ResourceCount = 0.0f;

        public MineResourceState()
        {
        }

        public MineResourceState(string newName, MineType newMineType, float newResourceCount)
        {
            Name = newName;

            MineType = newMineType;

            ResourceCount = newResourceCount;
        }
    }
}
