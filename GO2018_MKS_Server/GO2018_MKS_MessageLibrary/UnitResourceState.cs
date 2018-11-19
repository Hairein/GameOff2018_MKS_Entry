using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static GO2018_MKS_MessageLibrary.MessageLibraryUtitlity;

namespace GO2018_MKS_MessageLibrary
{
    public class UnitResourceState
    {
        public string Name = string.Empty;

        public UnitType UnitType = UnitType.breeder;

        public WorldCoordinate Position = new WorldCoordinate();
         
        public float FoodResourceCount = 0.0f;
        public float TechResourceCount = 0.0f;

        public UnitResourceState()
        {
        }

        public UnitResourceState(string newName, UnitType newUnitType, WorldCoordinate newPosition, float newFoodResourceCount, float newTechResourceCount)
        {
            Name = newName;

            UnitType = newUnitType;

            Position = newPosition;

            FoodResourceCount = newFoodResourceCount;
            TechResourceCount = newTechResourceCount;
        }
    }
}
