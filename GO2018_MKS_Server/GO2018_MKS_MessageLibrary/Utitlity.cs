using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GO2018_MKS_MessageLibrary
{
    public static class MessageLibraryUtitlity
    {
        public static string[] SessionMapNames =
        {
            "Morpholite",
            "Sunset",
            "Overlord"
        };

        public enum SessionTeam
        {
            blue,
            orange
        };

        public static int[] SessionDurationSeconds =
        {
            150,    // 2.5 * 60
            300,    // 5.0 * 60
            450,    // 7.5 * 60
            600     // 10.0 * 60
        };

        public enum SessionState
        {
            none,
            waiting,
            ingame,
            ending
        };

        public enum SessionResult
        {
            pending,
            won,
            lost
        };

        public enum UnitType
        {
            breeder,
            drone
        };

        public enum MineType
        {
            food,
            tech
        };
    }
}
