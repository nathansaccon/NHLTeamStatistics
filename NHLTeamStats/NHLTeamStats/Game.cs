using System;

namespace NHLTeamStats
{
    public enum Arena{
        HOME,
        AWAY
    }
    [Serializable]
    public class Game
    {
        public Arena arena;
        public int goalsFor;
        public int goalsAgainst;

        public int teamPoints; //"2, 1, 0"

        public int shotsFor;
        public int shotsAgainst;

        public int PenaltyMinutes;
        public int PowerplayGoals;
        public int PowerplayOpportunities;

        public float corsiForPercent;

        public Game()
        {

        }
    }
}