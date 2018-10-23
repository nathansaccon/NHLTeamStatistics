using System;

/* Nathan Saccon NHLTeamStats Project
 *          Date Started: October 21, 2018: Created project from scratch
 * 
 */

namespace NHLTeamsLib
{
    public enum Arena{
        HOME,
        AWAY
    }
    [Serializable]
    public class Game
    {
        private Arena arena;
        private int goalsFor;
        private int goalsAgainst;

        private int teamPoints; //"2, 1, 0"

        private int shotsFor;
        private int shotsAgainst;

        private int penaltyMinutes;
        private int powerplayGoals;
        private int powerplayOpportunities;

        private float corsiForPercent;

        public Arena Arena { get => arena; set => arena = value; }
        public int GoalsFor { get => goalsFor; set => goalsFor = value; }
        public int GoalsAgainst { get => goalsAgainst; set => goalsAgainst = value; }
        public int TeamPoints { get => teamPoints; set => teamPoints = value; }
        public int ShotsFor { get => shotsFor; set => shotsFor = value; }
        public int ShotsAgainst { get => shotsAgainst; set => shotsAgainst = value; }
        public int PenaltyMinutes { get => penaltyMinutes; set => penaltyMinutes = value; }
        public int PowerplayGoals { get => powerplayGoals; set => powerplayGoals = value; }
        public int PowerplayOpportunities { get => powerplayOpportunities; set => powerplayOpportunities = value; }
        public float CorsiForPercent { get => corsiForPercent; set => corsiForPercent = value; }

        public Game()
        {

        }
    }
}