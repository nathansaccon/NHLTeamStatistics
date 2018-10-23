using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* Nathan Saccon NHLTeamStats Project
 *          Date Started: October 21, 2018: Created project from scratch
 * 
 */

namespace NHLTeamStats
{
    [Serializable]
    class NHLTeam
    {
        #region Class Variables

        string name;
        string abbreviation;
        List<Game> gamelog;

        public string Name { get => name; set => name = value; }
        public string Abbreviation { get => abbreviation; set => abbreviation = value; }
        public List<Game> Gamelog { get => gamelog; set => gamelog = value; }

        #endregion

        #region Constructor

        public NHLTeam()
        {

        }

        #endregion

        #region Averages

        #region Goals For

        /// <summary>
        /// Returns the average number of goals for per game this season.
        /// </summary>
        /// <returns></returns>
        public float GoalsForPerGame()
        {
            float totalGoals = 0;
            foreach (Game game in Gamelog)
            {
                totalGoals += game.GoalsFor;
            }
            return totalGoals / Gamelog.Count;
        }

        /// <summary>
        /// Returns the average number of goals for per game for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float GoalsForPerGame(float lastXGames)
        {
            float totalGoals = 0;

            if (lastXGames >= Gamelog.Count)
            {
                return GoalsForPerGame();
            }
            else
            {
                int startingIndex = Gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    totalGoals += Gamelog[startingIndex].GoalsFor;
                    startingIndex++;
                }
            }
            return totalGoals / lastXGames;
        }

        /// <summary>
        /// Returns the average number of goals for per home game this season.
        /// </summary>
        /// <returns></returns>
        public float HomeGoalsForPerGame()
        {
            float totalGoals = 0;
            float numOfGames = 0;
            foreach (Game game in Gamelog)
            {
                if (game.Arena == Arena.HOME)
                {
                    totalGoals += game.GoalsFor;
                    numOfGames++;
                }
            }
            return totalGoals / numOfGames;
        }

        /// <summary>
        /// Returns the average number of goals for per away game this season.
        /// </summary>
        /// <returns></returns>
        public float AwayGoalsForPerGame()
        {
            float totalGoals = 0;
            float numOfGames = 0;
            foreach (Game game in Gamelog)
            {
                if (game.Arena == Arena.AWAY)
                {
                    totalGoals += game.GoalsFor;
                    numOfGames++;
                }
            }
            return totalGoals / numOfGames;
        }

        #endregion

        #region Goals Against

        /// <summary>
        /// Returns the average number of goals against per game this season.
        /// </summary>
        /// <returns></returns>
        public float GoalsAgainstPerGame()
        {
            float totalGoals = 0;
            foreach (Game game in Gamelog)
            {
                totalGoals += game.GoalsAgainst;
            }
            return totalGoals / Gamelog.Count;
        }

        /// <summary>
        /// Returns the average number of goals against per game for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float GoalsAgainstPerGame(float lastXGames)
        {
            float totalGoals = 0;

            if (lastXGames >= Gamelog.Count)
            {
                return GoalsAgainstPerGame();
            }
            else
            {
                int startingIndex = Gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    totalGoals += Gamelog[startingIndex].GoalsAgainst;
                    startingIndex++;
                }
            }
            return totalGoals / lastXGames;
        }

        /// <summary>
        /// Returns the average number of goals against per home game this season.
        /// </summary>
        /// <returns></returns>
        public float HomeGoalsAgainstPerGame()
        {
            float totalGoals = 0;
            float numOfGames = 0;
            foreach (Game game in Gamelog)
            {
                if (game.Arena == Arena.HOME)
                {
                    totalGoals += game.GoalsAgainst;
                    numOfGames++;
                }
            }
            return totalGoals / numOfGames;
        }

        /// <summary>
        /// Returns the average number of goals against per away game this season.
        /// </summary>
        /// <returns></returns>
        public float AwayGoalsAgainstPerGame()
        {
            float totalGoals = 0;
            float numOfGames = 0;
            foreach (Game game in Gamelog)
            {
                if (game.Arena == Arena.AWAY)
                {
                    totalGoals += game.GoalsAgainst;
                    numOfGames++;
                }
            }
            return totalGoals / numOfGames;
        }

        #endregion

        #region Shots For

        /// <summary>
        /// Returns the average number of shots per game this season.
        /// </summary>
        /// <returns></returns>
        public float ShotsForPerGame()
        {
            float totalShots = 0;
            foreach (Game game in Gamelog)
            {
                totalShots += game.ShotsFor;
            }
            return totalShots / Gamelog.Count;
        }

        /// <summary>
        /// Returns the average number of shots per game for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float ShotsForPerGame(float lastXGames)
        {
            float totalShots = 0;

            if (lastXGames >= Gamelog.Count)
            {
                return ShotsForPerGame();
            }
            else
            {
                int startingIndex = Gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    totalShots += Gamelog[startingIndex].ShotsFor;
                    startingIndex++;
                }
            }
            return totalShots / lastXGames;
        }

        /// <summary>
        /// Returns the average number of shots for per home game this season.
        /// </summary>
        /// <returns></returns>
        public float HomeShotsForPerGame()
        {
            float totalShots = 0;
            float numOfGames = 0;
            foreach (Game game in Gamelog)
            {
                if (game.Arena == Arena.HOME)
                {
                    totalShots += game.ShotsFor;
                    numOfGames++;
                }
            }
            return totalShots / numOfGames;
        }
        /// <summary>
        /// Returns the average number of shots for per away game this season.
        /// </summary>
        /// <returns></returns>
        public float AwayShotsForPerGame()
        {
            float totalShots = 0;
            float numOfGames = 0;
            foreach (Game game in Gamelog)
            {
                if (game.Arena == Arena.AWAY)
                {
                    totalShots += game.ShotsFor;
                    numOfGames++;
                }
            }
            return totalShots / numOfGames;
        }

        #endregion

        #region Shots Against

        /// <summary>
        /// Returns the average number of shots against per game this season.
        /// </summary>
        /// <returns></returns>
        public float ShotsAgainstPerGame()
        {
            float totalShots = 0;
            foreach (Game game in Gamelog)
            {
                totalShots += game.ShotsAgainst;
            }
            return totalShots / Gamelog.Count;
        }

        /// <summary>
        /// Returns the average number of shots against per game for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float ShotsAgainstPerGame(float lastXGames)
        {
            float totalShots = 0;

            if (lastXGames >= Gamelog.Count)
            {
                return ShotsAgainstPerGame();
            }
            else
            {
                int startingIndex = Gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    totalShots += Gamelog[startingIndex].ShotsAgainst;
                    startingIndex++;
                }
            }
            return totalShots / lastXGames;
        }

        /// <summary>
        /// Returns the average number of shots against per home game this season.
        /// </summary>
        /// <returns></returns>
        public float HomeShotsAgainstPerGame()
        {
            float totalShots = 0;
            float numOfGames = 0;
            foreach (Game game in Gamelog)
            {
                if (game.Arena == Arena.HOME)
                {
                    totalShots += game.ShotsAgainst;
                    numOfGames++;
                }
            }
            return totalShots / numOfGames;
        }

        /// <summary>
        /// Returns the average number of shots against per away game this season.
        /// </summary>
        /// <returns></returns>
        public float AwayShotsAgainstPerGame()
        {
            float totalShots = 0;
            float numOfGames = 0;
            foreach (Game game in Gamelog)
            {
                if (game.Arena == Arena.AWAY)
                {
                    totalShots += game.ShotsAgainst;
                    numOfGames++;
                }
            }
            return totalShots / numOfGames;
        }

        #endregion

        #region Penalty Minutes

        /// <summary>
        /// Returns the average number of PIM per game this season.
        /// </summary>
        /// <returns></returns>
        public float PIMPerGame()
        {
            float totalPIM = 0;
            foreach (Game game in Gamelog)
            {
                totalPIM += game.PenaltyMinutes;
            }
            return totalPIM / Gamelog.Count;
        }

        #endregion

        #region Win Rate

        /// <summary>
        /// Returns the percentage of time a team gets two points.
        /// </summary>
        /// <returns></returns>
        public float WinRate()
        {
            float gamesWon = 0;
            foreach (Game game in Gamelog)
            {
                if (game.TeamPoints == 2)
                {
                    gamesWon++;
                }
            }
            return gamesWon / Gamelog.Count;
        }

        /// <summary>
        /// Returns the percentage of time a team gets two points in th last X games
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float WinRate(float lastXGames)
        {
            float totalWins = 0;

            if (lastXGames >= Gamelog.Count)
            {
                return WinRate();
            }
            else
            {
                int startingIndex = Gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    if(Gamelog[startingIndex].TeamPoints == 2)
                    {
                        totalWins++;
                    }
                    startingIndex++;
                }
            }
            return totalWins / lastXGames;
        }

        /// <summary>
        /// Returns the percentage of time a team gets two points at home.
        /// </summary>
        /// <returns></returns>
        public float HomeWinRate()
        {
            float gamesPlayed = 0;
            float gamesWon = 0;
            foreach(Game game in Gamelog)
            {
                if(game.Arena == Arena.HOME)
                {
                    gamesPlayed++;
                    if (game.TeamPoints == 2)
                    {
                        gamesWon++;
                    }
                }
            }
            return gamesWon / gamesPlayed;
        }

        /// <summary>
        /// Returns the percentage of time a team gets two points while away.
        /// </summary>
        /// <returns></returns>
        public float AwayWinRate()
        {
            float gamesPlayed = 0;
            float gamesWon = 0;
            foreach (Game game in Gamelog)
            {
                if (game.Arena == Arena.AWAY)
                {
                    gamesPlayed++;
                    if (game.TeamPoints == 2)
                    {
                        gamesWon++;
                    }
                }
            }
            return gamesWon / gamesPlayed;
        }

        #endregion

        #region Corsi For

        /// <summary>
        /// Returns the average corsi for percent for a team.
        /// </summary>
        /// <returns></returns>
        public float CorsiPerGame()
        {
            float totalCorsi = 0;
            foreach(Game game in Gamelog)
            {
                totalCorsi += game.CorsiForPercent;
            }
            return totalCorsi / Gamelog.Count;
        }

        /// <summary>
        /// Returns the average Corsi for, for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float CorsiPerGame(float lastXGames)
        {
            float totalCorsi = 0;
            if (lastXGames >= Gamelog.Count)
            {
                return ShotsForPerGame();
            }
            else
            {
                int startingIndex = Gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    totalCorsi += Gamelog[startingIndex].CorsiForPercent;
                    startingIndex++;
                }
            }
            return totalCorsi / lastXGames;
        }

        /// <summary>
        /// Returns the average home corsi for percent for a team.
        /// </summary>
        /// <returns></returns>
        public float HomeCorsiPerGame()
        {
            float totalCorsi = 0;
            float totalGames = 0;
            foreach (Game game in Gamelog)
            {
                if (game.Arena == Arena.HOME)
                {
                    totalCorsi += game.CorsiForPercent;
                    totalGames++;
                }
            }
            return totalCorsi / totalGames;
        }

        /// <summary>
        /// Returns the average away corsi for percent for a team.
        /// </summary>
        /// <returns></returns>
        public float AwayCorsiPerGame()
        {
            float totalCorsi = 0;
            float totalGames = 0;
            foreach (Game game in Gamelog)
            {
                if (game.Arena == Arena.AWAY)
                {
                    totalCorsi += game.CorsiForPercent;
                    totalGames++;
                }
            }
            return totalCorsi / totalGames;
        }

        #endregion

        #endregion

        #region Totals Stats

        /// <summary>
        /// Returns the total number of goals a team has scored.
        /// </summary>
        /// <returns></returns>
        public int GoalsScored()
        {
            int numberOfGoals = 0;
            foreach(Game game in Gamelog)
            {
                numberOfGoals += game.GoalsFor;
            }
            return numberOfGoals;
        }

        /// <summary>
        /// Returns the total number of goals against a team has allowed.
        /// </summary>
        /// <returns></returns>
        public int GoalsAllowed()
        {
            int totalGoalsAllowed = 0;
            foreach(Game game in Gamelog)
            {
                totalGoalsAllowed += game.GoalsAgainst;
            }
            return totalGoalsAllowed;
        }

        /// <summary>
        /// Returns the number of power play goals a team scored this season.
        /// </summary>
        /// <returns></returns>
        public int PowerPlayGoals()
        {
            int totalPPGs = 0;
            foreach(Game game in Gamelog)
            {
                totalPPGs += game.PowerplayGoals;
            }
            return totalPPGs;
        }

        /// <summary>
        /// Returns a team's total number of power play opportunities.
        /// </summary>
        /// <returns></returns>
        public int PowerPlayOpportunities()
        {
            int powerPlayOpportunities = 0;
            foreach(Game game in Gamelog)
            {
                powerPlayOpportunities += game.PowerplayOpportunities;
            }
            return powerPlayOpportunities;
        }

        #endregion

        #region Other Stats

        /// <summary>
        /// Returns percent of time a team scores even strength.
        /// </summary>
        /// <returns></returns>
        public float EvenStrengthGoalPercent()
        {
            return 1 - (float)PowerPlayGoals() / (float)GoalsScored();
        }

        /// <summary>
        /// Returns percentage of time a team scores on the powerplay.
        /// </summary>
        /// <returns></returns>
        public float PowerPlayPercent()
        {
            return (float)PowerPlayGoals() / (float)PowerPlayOpportunities();
        }

        #endregion
        
    }
}
