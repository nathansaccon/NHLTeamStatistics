using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

/* Nathan Saccon NHLTeamStats Project
 *          Date Started: October 21, 2018: Created project from scratch
 *                        Oct 23, 2018: Refactored to update stat variables in one loop.
 * 
 */

namespace NHLTeamsLib
{
    [Serializable]
    public class NHLTeam
    {
        #region Class Variables

        private string name;
        private string abbreviation;
        private List<Game> gamelog;

        const int GAMES_PER_SEASON = 82;

        public string Name { get => name; set => name = value; }
        public string Abbreviation { get => abbreviation; set => abbreviation = value; }

        #endregion

        #region Next Opponent Variables

        private string nextOpponentName;
        private string nextGameDate;
        private Arena nextGameArena;

        public string NextOpponentName { get => nextOpponentName; }
        public string NextGameDate { get => nextGameDate; }
        public Arena NextGameArena { get => nextGameArena; }

        #endregion

        #region Stat Variables

        public float GoalsForPerGame;
        public float GoalsForStandardDeviation;
        public float HomeGoalsForPerGame;
        public float HomeGoalsForStandardDeviation;
        public float AwayGoalsForPerGame;
        public float AwayGoalsForStandardDeviation;

        public float GoalsAgainstPerGame;
        public float GoalsAgainstStandardDeviation;
        public float HomeGoalsAgainstPerGame;
        public float HomeGoalsAgainstStandardDeviation;
        public float AwayGoalsAgainstPerGame;
        public float AwayGoalsAgainstStandardDeviation;

        public float ShotsForPerGame;
        public float ShotsForStandardDeviation;
        public float HomeShotsForPerGame;
        public float HomeShotsForStandardDeviation;
        public float AwayShotsForPerGame;
        public float AwayShotsForStandardDeviation;

        public float ShotsAgainstPerGame;
        public float ShotsAgainstStandardDeviation;
        public float HomeShotsAgainstPerGame;
        public float HomeShotsAgainstStandardDeviation;
        public float AwayShotsAgainstPerGame;
        public float AwayShotsAgainstStandardDeviation;

        public float PIMPerGame;
        public float PIMStandardDeviation;
        public float HomePIMPerGame;
        public float HomePIMStandardDeviation;
        public float AwayPIMPerGame;
        public float AwayPIMStandardDeviation;

        public float WinRate;
        public float HomeWinRate;
        public float AwayWinRate;

        public float CorsiFor;
        public float CorsiStandardDeviation;
        public float HomeCorsiFor;
        public float HomeCorsiStandardDeviation;
        public float AwayCorsiFor;
        public float AwayCorsiStandardDeviation;

        public float EvenStrengthGoalPercent;
        public float HomeEvenStrengthGoalPercent;
        public float AwayEvenStrengthGoalPercent;

        public float PowerPlayPercent;
        public float HomePowerPlayPercent;
        public float AwayPowerPlayPercent;

        #endregion

        #region Constructor

        public NHLTeam()
        {

        }

        #endregion

        #region Download String
        /// <summary>
        /// Returns the html string of the given web address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static string DownloadString(string address)
        {
            WebClient client = new WebClient();
            return client.DownloadString(address);
        }

        #endregion

        #region Populate Team (Update)

        /// <summary>
        /// Populates the team's gamelog and statistic variables
        /// </summary>
        public void PopulateTeam(int year)
        {
            PopulateGamelog(year);
            PopulateStats();
        }

        #region Populate Gamelog (Pirvate Methods)

        /// <summary>
        /// Populates this team's gamelog based on the chosen year
        /// </summary>
        /// <param name="year"></param>
        private void PopulateGamelog(int year)
        {
            gamelog = TeamStatPageToGameList(year);
        }

        /// <summary>
        /// Returns a list of all the games a team (by abbreviation) played in a year.
        /// </summary>
        /// <param name="abbreviation"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private List<Game> TeamStatPageToGameList(int year)
        {
            string teamStatURL = "https://www.hockey-reference.com/teams/" + Abbreviation + "/" + year.ToString() + "_gamelog.html";
            string page = DownloadString(teamStatURL);

            List<Game> allGamesPlayed = new List<Game>();

            for (int i = 1; i < GAMES_PER_SEASON; i++)
            {
                string gameSoup = page.Split(new string[] { "tm_gamelog_rs." }, StringSplitOptions.None)[i];
                string[] row = gameSoup.Split('>');
                Game thisGame = RowToGame(row);
                if (thisGame == null)
                {
                    break;
                }
                else
                {
                    allGamesPlayed.Add(thisGame);
                }
            }
            return allGamesPlayed;
        }

        /// <summary>
        /// Returns a game from a row in the HTML file
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Game RowToGame(String[] row)
        {
            Game thisGame = new Game();
            // Check Home/Away
            string arena = row[8].Split('<')[0];

            if (arena == "@")
            {
                thisGame.Arena = Arena.AWAY;
            }
            else
            {
                thisGame.Arena = Arena.HOME;
            }
            // Goals
            string goalsFor = row[14].Split('<')[0];
            if (goalsFor == "")
            {
                nextOpponentName = row[9].Split('<')[0];
                nextGameDate = row[4].Split('<')[0];
                arena = row[6].Split('<')[0];
                if (arena == "@")
                {
                    nextGameArena = Arena.AWAY;
                }
                else
                {
                    nextGameArena = Arena.HOME;
                }
                return null;
            }
            string goalsAgainst = row[16].Split('<')[0];

            thisGame.GoalsFor = Convert.ToInt32(goalsFor);
            thisGame.GoalsAgainst = Convert.ToInt32(goalsAgainst);
            // Points
            string overallResult = row[18].Split('<')[0];
            string context = row[20].Split('<')[0];

            thisGame.TeamPoints = ResultStringsToValue(overallResult, context);
            // Shots
            string shotsFor = row[24].Split('<')[0];
            string shotsAgainst = row[36].Split('<')[0];

            thisGame.ShotsFor = Convert.ToInt32(shotsFor);
            thisGame.ShotsAgainst = Convert.ToInt32(shotsAgainst);
            // Powerplay
            string PenaltyMinutes = row[26].Split('<')[0];
            string PPGoals = row[28].Split('<')[0];
            string PPOpportunities = row[30].Split('<')[0];

            thisGame.PenaltyMinutes = Convert.ToInt32(PenaltyMinutes);
            thisGame.PowerplayGoals = Convert.ToInt32(PPGoals);
            thisGame.PowerplayOpportunities = Convert.ToInt32(PPOpportunities);
            // Advanced
            string corsiFor = row[52].Split('<')[0];

            thisGame.CorsiForPercent = float.Parse(corsiFor);


            return thisGame;
        }

        /// <summary>
        /// Returns the number of points a team achieved based on W, OT, SO.
        /// </summary>
        /// <param name="overallResult"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        private int ResultStringsToValue(string overallResult, string context)
        {
            int pointsEarned = 0;

            if (overallResult == "W")
            {
                pointsEarned = 2;
            }
            else if (context == "OT" || context == "SO")
            {
                pointsEarned = 1;
            }

            return pointsEarned;
        }

        #endregion

        #region Populate Stat Variables (Private Methods)

        private void PopulateStats()
        {
            PopulateSeasonVariables();
            PopulateArenaVariables(Arena.HOME);
            PopulateArenaVariables(Arena.AWAY);
        }

        /// <summary>
        /// Populates the season average class variables
        /// </summary>
        private void PopulateSeasonVariables()
        {
            List<float> goalsFor = new List<float>();
            List <float> goalsAgainst = new List<float>();
            List <float> ppgs = new List<float>();
            List <float> ppOpportunities = new List<float>();
            List <float> shotsFor = new List<float>();
            List <float> shotsAgainst = new List<float>();
            List <float> penaltyMins = new List<float>();
            List<float> corsiFor = new List<float>();
            List<float> wins = new List<float>();

            foreach (Game game in gamelog)
            {
                goalsFor.Add(game.GoalsFor);
                goalsAgainst.Add(game.GoalsAgainst);
                ppgs.Add(game.PowerplayGoals);
                ppOpportunities.Add(game.PowerplayOpportunities);
                shotsFor.Add(game.ShotsFor);
                shotsAgainst.Add(game.ShotsAgainst);
                penaltyMins.Add(game.PenaltyMinutes);
                corsiFor.Add(game.CorsiForPercent);
                if(game.TeamPoints == 2)
                {
                    wins.Add(1);
                }
                else
                {
                    wins.Add(0);
                }
            }
            //Averages
            GoalsForPerGame = goalsFor.Average();
            GoalsAgainstPerGame = goalsAgainst.Average();
            EvenStrengthGoalPercent = 1 - ppgs.Average();
            PowerPlayPercent = ppgs.Average();
            ShotsForPerGame = shotsFor.Average();
            ShotsAgainstPerGame = shotsAgainst.Average();
            PIMPerGame = penaltyMins.Average();
            CorsiFor = corsiFor.Average();
            WinRate = wins.Average();
            // Standard Deviation Stats
            GoalsForStandardDeviation = StandardDeviationOf(goalsFor);
            GoalsAgainstStandardDeviation = StandardDeviationOf(goalsAgainst);
            ShotsForStandardDeviation = StandardDeviationOf(shotsFor);
            ShotsAgainstStandardDeviation = StandardDeviationOf(shotsAgainst);
            PIMStandardDeviation = StandardDeviationOf(penaltyMins);
            CorsiStandardDeviation = StandardDeviationOf(corsiFor);
        }

        /// <summary>
        /// Poplates the home/away stat class variables
        /// </summary>
        /// <param name="arena"></param>
        private void PopulateArenaVariables(Arena arena)
        {
            List<float> goalsFor = new List<float>();
            List<float> goalsAgainst = new List<float>();
            List<float> ppgs = new List<float>();
            List<float> ppOpportunities = new List<float>();
            List<float> shotsFor = new List<float>();
            List<float> shotsAgainst = new List<float>();
            List<float> penaltyMins = new List<float>();
            List<float> corsiFor = new List<float>();
            List<float> wins = new List<float>();

            foreach (Game game in gamelog)
            {
                if (game.Arena == arena)
                {
                    goalsFor.Add(game.GoalsFor);
                    goalsAgainst.Add(game.GoalsAgainst);
                    ppgs.Add(game.PowerplayGoals);
                    ppOpportunities.Add(game.PowerplayOpportunities);
                    shotsFor.Add(game.ShotsFor);
                    shotsAgainst.Add(game.ShotsAgainst);
                    penaltyMins.Add(game.PenaltyMinutes);
                    corsiFor.Add(game.CorsiForPercent);
                    if (game.TeamPoints == 2)
                    {
                        wins.Add(1);
                    }else
                    {
                        wins.Add(0);
                    }
                }
            }

            if (arena == Arena.HOME)
            {
                // Averages
                HomeGoalsForPerGame = goalsFor.Average();
                HomeGoalsAgainstPerGame = goalsAgainst.Average();
                HomeGoalsForStandardDeviation = StandardDeviationOf(goalsFor);
                HomeEvenStrengthGoalPercent = 1 - ppgs.Average();
                HomePowerPlayPercent = ppgs.Average();
                HomeShotsForPerGame = shotsFor.Average();
                HomeShotsAgainstPerGame = shotsAgainst.Average();
                HomePIMPerGame = penaltyMins.Average();
                HomeCorsiFor = corsiFor.Average();
                HomeWinRate = wins.Average();
                // Standard Deviation Stats
                HomeGoalsForStandardDeviation = StandardDeviationOf(goalsFor);
                HomeGoalsAgainstStandardDeviation = StandardDeviationOf(goalsAgainst);
                HomeShotsForStandardDeviation = StandardDeviationOf(shotsFor);
                HomeShotsAgainstStandardDeviation = StandardDeviationOf(shotsAgainst);
                HomePIMStandardDeviation = StandardDeviationOf(penaltyMins);
                HomeCorsiStandardDeviation = StandardDeviationOf(corsiFor);
            } else
            {
                //Averages
                AwayGoalsForPerGame = goalsFor.Average();
                AwayGoalsAgainstPerGame = goalsAgainst.Average();
                AwayGoalsForStandardDeviation = StandardDeviationOf(goalsFor);
                AwayEvenStrengthGoalPercent = 1 - ppgs.Average();
                AwayPowerPlayPercent = ppgs.Average();
                AwayShotsForPerGame = shotsFor.Average();
                AwayShotsAgainstPerGame = shotsAgainst.Average();
                AwayPIMPerGame = penaltyMins.Average();
                AwayCorsiFor = corsiFor.Average();
                AwayWinRate = wins.Average();
                // Standard Deviation Stats
                AwayGoalsForStandardDeviation = StandardDeviationOf(goalsFor);
                AwayGoalsAgainstStandardDeviation = StandardDeviationOf(goalsAgainst);
                AwayShotsForStandardDeviation = StandardDeviationOf(shotsFor);
                AwayShotsAgainstStandardDeviation = StandardDeviationOf(shotsAgainst);
                AwayPIMStandardDeviation = StandardDeviationOf(penaltyMins);
                AwayCorsiStandardDeviation = StandardDeviationOf(corsiFor);
            }
        }

        #endregion

        #endregion

        #region Last X Methods

        /// <summary>
        /// Returns the average number of goals for per game for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float GoalsForLastX(float lastXGames)
        {
            float totalGoals = 0;

            if (lastXGames >= gamelog.Count)
            {
                return GoalsForPerGame;
            }
            else
            {
                int startingIndex = gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    totalGoals += gamelog[startingIndex].GoalsFor;
                    startingIndex++;
                }
            }
            return totalGoals / lastXGames;
        }

        /// <summary>
        /// Returns the average number of goals against per game for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float GoalsAgainstLastX(float lastXGames)
        {
            float totalGoals = 0;

            if (lastXGames >= gamelog.Count)
            {
                return GoalsAgainstPerGame;
            }
            else
            {
                int startingIndex = gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    totalGoals += gamelog[startingIndex].GoalsAgainst;
                    startingIndex++;
                }
            }
            return totalGoals / lastXGames;
        }

        /// <summary>
        /// Returns the average number of shots per game for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float ShotsForLastX(float lastXGames)
        {
            float totalShots = 0;

            if (lastXGames >= gamelog.Count)
            {
                return ShotsForPerGame;
            }
            else
            {
                int startingIndex = gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    totalShots += gamelog[startingIndex].ShotsFor;
                    startingIndex++;
                }
            }
            return totalShots / lastXGames;
        }

        /// <summary>
        /// Returns the average number of shots against per game for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float ShotsAgainstLastX(float lastXGames)
        {
            float totalShots = 0;

            if (lastXGames >= gamelog.Count)
            {
                return ShotsAgainstPerGame;
            }
            else
            {
                int startingIndex = gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    totalShots += gamelog[startingIndex].ShotsAgainst;
                    startingIndex++;
                }
            }
            return totalShots / lastXGames;
        }

        /// <summary>
        /// Returns the percentage of time a team gets two points in th last X games
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float WinRateLastX(float lastXGames)
        {
            float totalWins = 0;

            if (lastXGames >= gamelog.Count)
            {
                return WinRate;
            }
            else
            {
                int startingIndex = gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    if (gamelog[startingIndex].TeamPoints == 2)
                    {
                        totalWins++;
                    }
                    startingIndex++;
                }
            }
            return totalWins / lastXGames;
        }

        /// <summary>
        /// Returns the average Corsi for, for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float CorsiLastX(float lastXGames)
        {
            float totalCorsi = 0;
            if (lastXGames >= gamelog.Count)
            {
                return CorsiFor;
            }
            else
            {
                int startingIndex = gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    totalCorsi += gamelog[startingIndex].CorsiForPercent;
                    startingIndex++;
                }
            }
            return totalCorsi / lastXGames;
        }

        #endregion

        #region Math Methods

        /// <summary>
        /// Returns the standard deviation of the numbers in the given list.
        /// </summary>
        /// <param name="myNumbers"></param>
        /// <returns></returns>
        private float StandardDeviationOf(List<float> myNumbers)
        {
            float average = myNumbers.Average();
            float sumOfSquaresOfDifferences = myNumbers.Select(val => (val - average) * (val - average)).Sum();
            float standardDeviation = (float)Math.Sqrt(sumOfSquaresOfDifferences / myNumbers.Count);

            return standardDeviation;
        }

        #endregion

        #region Static Methods

        /// <summary>
        /// Return the NHLTeam that thisTeam will play next
        /// </summary>
        /// <param name="thisTeam"></param>
        /// <param name="otherTeams"></param>
        /// <returns></returns>
        public static NHLTeam NextOpponent(NHLTeam thisTeam, NHLTeam[] otherTeams)
        {
            NHLTeam opponent = null;
            foreach(NHLTeam team in otherTeams)
            {
                if(team.Name == thisTeam.NextOpponentName)
                {
                    opponent = team;
                    break;
                }
            }
            return opponent;
        }

        #endregion
    }
}
