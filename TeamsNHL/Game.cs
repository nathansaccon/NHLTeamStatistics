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

        // Team and Player
        private Arena arena;
        private string date;
        private int goalsFor;
        private int shotsFor;
        private int penaltyMinutes;
        private int powerplayGoals;
        private string opponent;
        // Team
        private bool played;
        private int goalsAgainst;
        private int teamPoints; //"2, 1, 0"
        private int shotsAgainst;
        private int powerplayOpportunities;
        private float corsiForPercent;
        // Skater
        private int assists;
        private int plusMinus;
        private string timeOnIce;
        private int hits;
        private int blocks;

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
        public string Date { get => date; set => date = value; }
        public string Opponent { get => opponent; set => opponent = value; }
        public bool Played { get => played; set => played = value; }
        public int Assists { get => assists; set => assists = value; }
        public int PlusMinus { get => plusMinus; set => plusMinus = value; }
        public string TimeOnIce { get => timeOnIce; set => timeOnIce = value; }
        public int Hits { get => hits; set => hits = value; }
        public int Blocks { get => blocks; set => blocks = value; }

        public Game()
        {

        }
    }
}