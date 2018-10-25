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
    public class Team
    {
        #region Class Variables

        internal string name;
        private string abbreviation;
        private List<Game> gamelog;
        private List<Game> futureGamelog;

        public string Name { get => name;}
        public string Abbreviation { get => abbreviation;}

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

        public Team(string name, string abbreviation)
        {
            this.name = name;
            this.abbreviation = abbreviation;
            this.PopulateTeam(NHL.YEAR);
        }

        #endregion

        #region Download String
        /// <summary>
        /// Returns the html string of the given web address.
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        internal static string DownloadString(string address)
        {
            WebClient client = new WebClient();
            return client.DownloadString(address);
        }

        #endregion

        #region Populate Team (Update)

        /// <summary>
        /// Populates the team's gamelog and statistic variables
        /// </summary>
        private void PopulateTeam(int year)
        {
            PopulateGamelogs(year);
            PopulateStats();
        }

        #region Populate Gamelog (Pirvate Methods)

        ///// <summary>
        ///// Populates this team's gamelog based on the chosen year
        ///// </summary>
        ///// <param name="year"></param>
        //private void PopulateGamelog(int year)
        //{
        //    gamelog = TeamStatPageToGameList(year);
        //}

        /// <summary>
        /// Returns a list of all the games a team (by abbreviation) played in a year.
        /// </summary>
        /// <param name="abbreviation"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private void PopulateGamelogs(int year)
        {
            string teamStatURL = "https://www.hockey-reference.com/teams/" + Abbreviation + "/" + year.ToString() + "_gamelog.html";
            string page = DownloadString(teamStatURL);

            List<Game> allGamesPlayed = new List<Game>();
            List<Game> futureGames = new List<Game>();

            for (int i = 1; i < NHL.GAMES_PER_SEASON+1; i++)
            {
                string gameSoup = page.Split(new string[] { "tm_gamelog_rs." }, StringSplitOptions.None)[i];
                string[] row = gameSoup.Split('>');
                Game thisGame = RowToGame(row);
                if (!thisGame.Played)
                {
                    futureGames.Add(thisGame);
                }
                else
                {
                    allGamesPlayed.Add(thisGame);
                }
            }
            gamelog = allGamesPlayed;
            futureGamelog = futureGames; 
        }

        /// <summary>
        /// Returns a game from a row in the HTML file
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private Game RowToGame(string[] row)
        {
            Game thisGame = new Game();
            bool completedGame = RowToStat(row[14]) != "";

            if (completedGame)
            {
                thisGame.Played = true;
                // Date
                string date = RowToStat(row[5]);

                thisGame.Date = date;
                // Check Home/Away
                string arena = RowToStat(row[8]);

                if (arena == "@")
                {
                    thisGame.Arena = Arena.AWAY;
                }
                else
                {
                    thisGame.Arena = Arena.HOME;
                }
                // Opponent
                string opponent = RowToStat(row[11]);

                thisGame.Opponent = opponent;
                // Goals
                string goalsFor = RowToStat(row[14]);
                string goalsAgainst = RowToStat(row[16]);

                thisGame.GoalsFor = Convert.ToInt32(goalsFor);
                thisGame.GoalsAgainst = Convert.ToInt32(goalsAgainst);
                // Points
                string overallResult = RowToStat(row[18]);
                string context = RowToStat(row[20]);

                thisGame.TeamPoints = ResultStringsToValue(overallResult, context);
                // Shots
                string shotsFor = RowToStat(row[24]);
                string shotsAgainst = RowToStat(row[36]);

                thisGame.ShotsFor = Convert.ToInt32(shotsFor);
                thisGame.ShotsAgainst = Convert.ToInt32(shotsAgainst);
                // Powerplay
                string PenaltyMinutes = RowToStat(row[26]);
                string PPGoals = RowToStat(row[28]);
                string PPOpportunities = RowToStat(row[30]);

                thisGame.PenaltyMinutes = Convert.ToInt32(PenaltyMinutes);
                thisGame.PowerplayGoals = Convert.ToInt32(PPGoals);
                thisGame.PowerplayOpportunities = Convert.ToInt32(PPOpportunities);
                // Advanced
                string corsiFor = RowToStat(row[52]);

                thisGame.CorsiForPercent = float.Parse(corsiFor);
            }else
            {
                thisGame.Played = false;
                string opponentName = RowToStat(row[9]);
                string arena = RowToStat(row[6]);
                string date = RowToStat(row[4]);

                if(arena == "@")
                {
                    thisGame.Arena = Arena.AWAY;
                }
                else
                {
                    thisGame.Arena = Arena.HOME;
                }
                thisGame.Date = date;
                thisGame.Opponent = opponentName;
            }


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

        /// <summary>
        /// Returns the value in a column.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        private string RowToStat(string column)
        {
            return column.Split('<')[0];
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

        /// <summary>
        /// Returns the average PIM, for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float PIMLastX(float lastXGames)
        {
            float totalPIM = 0;
            if (lastXGames >= gamelog.Count)
            {
                return PIMPerGame;
            }
            else
            {
                int startingIndex = gamelog.Count - (int)lastXGames;
                for (int i = 0; i < lastXGames; i++)
                {
                    totalPIM += gamelog[startingIndex].PenaltyMinutes;
                    startingIndex++;
                }
            }
            return totalPIM / lastXGames;
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

        #region Matchups

        /// <summary>
        /// Returns the matchups for a given date, with the home team at [0] and away at [1]
        /// </summary>
        /// <param name="allTeams"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public static List<Team[]> MatchupsByDate(Team[] allTeams, string date)
        {
            List<Team[]> matchupList = new List<Team[]>();
            List<string> teamsAdded = new List<string>();

            for (int i = 0; i < allTeams.Length; i++)
            {
                if (!teamsAdded.Contains(allTeams[i].name))
                {
                    foreach (Game game in allTeams[i].futureGamelog)
                    {
                        if (game.Date == date)
                        {
                            for (int j = 0; j < allTeams.Length; j++)
                            {
                                if (allTeams[j].name == game.Opponent)
                                {
                                    Team[] pair = new Team[2];
                                    if (game.Arena == Arena.HOME)
                                    {
                                        pair[0] = allTeams[i];
                                        pair[1] = allTeams[j];
                                    }
                                    else
                                    {
                                        pair[0] = allTeams[j];
                                        pair[1] = allTeams[i];
                                    }
                                    teamsAdded.Add(allTeams[i].name);
                                    teamsAdded.Add(allTeams[j].name);

                                    matchupList.Add(pair);
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
            }
            return matchupList;
        }

        #endregion


        /// <summary>
        /// Returns the list of all NHL teams.
        /// </summary>
        internal static Team[] AllTeams()
        {
            Team[] allTeams = new Team[NHL.NUMBER_OF_TEAMS];
            string teamTableData = NHL.HOMEPAGE.Split(new string[] { "selector_0" }, StringSplitOptions.None)[1];
            int initialRow = 2;
            for (int i = initialRow; i < initialRow + allTeams.Length; i++)
            {

                string rowData = teamTableData.Split(new string[] { "<option value=\"" }, StringSplitOptions.None)[i];
                string currentAbbreviation = rowData.Split(new string[] { "\"" }, StringSplitOptions.None)[0];
                string currentName = rowData.Split('>')[1].Split('<')[0];

                Team newTeam = new Team(currentName, currentAbbreviation);
                allTeams[i - initialRow] = newTeam;
            }
            return allTeams;
        }

        #region Outcome Predictors

        public static string TeamCategoryCompare(Team homeTeam, Team awayTeam)
        {
            int homeTeamPoints = 0;
            int awayTeamPoints = 0;
            
            //Goals for
            if(homeTeam.HomeGoalsForPerGame > awayTeam.AwayGoalsForPerGame)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.GoalsForPerGame > awayTeam.GoalsForPerGame)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.GoalsForLastX(5) > awayTeam.GoalsForLastX(5))
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            // Goals against
            if(homeTeam.HomeGoalsAgainstPerGame < awayTeam.AwayGoalsAgainstPerGame)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.GoalsAgainstPerGame < awayTeam.GoalsAgainstPerGame)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.GoalsAgainstLastX(5) < awayTeam.GoalsAgainstLastX(5))
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            // Shots for
            if(homeTeam.HomeShotsForPerGame > awayTeam.AwayShotsForPerGame)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.ShotsForPerGame > awayTeam.ShotsForPerGame)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.ShotsForLastX(5) > awayTeam.ShotsForLastX(5))
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            // Shots against
            if(homeTeam.HomeShotsAgainstPerGame < awayTeam.AwayShotsAgainstPerGame)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.ShotsAgainstPerGame < awayTeam.ShotsAgainstPerGame)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.ShotsAgainstLastX(5) < awayTeam.ShotsAgainstLastX(5))
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            // PIM
            if(homeTeam.HomePIMPerGame < awayTeam.AwayPIMPerGame)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.PIMPerGame < awayTeam.PIMPerGame)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.PIMLastX(5) < awayTeam.PIMLastX(5))
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            // Win Rate
            if(homeTeam.HomeWinRate > awayTeam.AwayWinRate)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if (homeTeam.WinRate > awayTeam.WinRate)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.WinRateLastX(5) > awayTeam.WinRateLastX(5))
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            // Corsi For
            if(homeTeam.HomeCorsiFor > awayTeam.AwayCorsiFor)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.CorsiFor > awayTeam.CorsiFor)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if (homeTeam.CorsiLastX(5) > awayTeam.CorsiLastX(5))
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            // Even Strength Goals
            if(homeTeam.HomeEvenStrengthGoalPercent > awayTeam.AwayEvenStrengthGoalPercent)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }
            if(homeTeam.EvenStrengthGoalPercent > awayTeam.EvenStrengthGoalPercent)
            {
                homeTeamPoints++;
            }else
            {
                awayTeamPoints++;
            }

            return homeTeam.Name + ": " + homeTeamPoints + "    |   " + awayTeam.Name + ": " + awayTeamPoints;

        }

        public static string TeamCategoryDifferenceCompare(Team homeTeam, Team awayTeam)
        {
            float homeTeamPoints = 0;
            float awayTeamPoints = 0;

            //Goals for
            if (homeTeam.HomeGoalsForPerGame > awayTeam.AwayGoalsForPerGame)
            {
                homeTeamPoints += homeTeam.HomeGoalsForPerGame - awayTeam.AwayGoalsForPerGame;
            }
            else
            {
                awayTeamPoints += awayTeam.AwayGoalsForPerGame - homeTeam.HomeGoalsForPerGame;
            }
            if (homeTeam.GoalsForPerGame > awayTeam.GoalsForPerGame)
            {
                homeTeamPoints += homeTeam.GoalsForPerGame - awayTeam.GoalsForPerGame;
            }
            else
            {
                awayTeamPoints += awayTeam.GoalsForPerGame - homeTeam.GoalsForPerGame;
            }
            if (homeTeam.GoalsForLastX(5) > awayTeam.GoalsForLastX(5))
            {
                homeTeamPoints += homeTeam.GoalsForLastX(5) - awayTeam.GoalsForLastX(5);
            }
            else
            {
                awayTeamPoints += awayTeam.GoalsForLastX(5) - homeTeam.GoalsForLastX(5);
            }
            // Goals against
            if (homeTeam.HomeGoalsAgainstPerGame < awayTeam.AwayGoalsAgainstPerGame)
            {
                homeTeamPoints += awayTeam.AwayGoalsAgainstPerGame - homeTeam.HomeGoalsAgainstPerGame;
            }
            else
            {
                awayTeamPoints += homeTeam.HomeGoalsAgainstPerGame - awayTeam.AwayGoalsAgainstPerGame;
            }
            if (homeTeam.GoalsAgainstPerGame < awayTeam.GoalsAgainstPerGame)
            {
                homeTeamPoints += awayTeam.GoalsAgainstPerGame - homeTeam.GoalsAgainstPerGame;
            }
            else
            {
                awayTeamPoints += homeTeam.GoalsAgainstPerGame - awayTeam.GoalsAgainstPerGame;
            }
            if (homeTeam.GoalsAgainstLastX(5) < awayTeam.GoalsAgainstLastX(5))
            {
                homeTeamPoints += awayTeam.GoalsAgainstLastX(5) - homeTeam.GoalsAgainstLastX(5);
            }
            else
            {
                awayTeamPoints += homeTeam.GoalsAgainstLastX(5) - awayTeam.GoalsAgainstLastX(5);
            }
            // Shots for
            if (homeTeam.HomeShotsForPerGame > awayTeam.AwayShotsForPerGame)
            {
                homeTeamPoints += (homeTeam.HomeShotsForPerGame - awayTeam.AwayShotsForPerGame)*0.2f;
            }
            else
            {
                awayTeamPoints += (awayTeam.AwayShotsForPerGame - homeTeam.HomeShotsForPerGame)*0.2f;
            }
            if (homeTeam.ShotsForPerGame > awayTeam.ShotsForPerGame)
            {
                homeTeamPoints += (homeTeam.ShotsForPerGame - awayTeam.ShotsForPerGame)*0.2f;
            }
            else
            {
                awayTeamPoints += (awayTeam.ShotsForPerGame - homeTeam.ShotsForPerGame)*0.2f;
            }
            if (homeTeam.ShotsForLastX(5) > awayTeam.ShotsForLastX(5))
            {
                homeTeamPoints += (homeTeam.ShotsForLastX(5) - awayTeam.ShotsForLastX(5))*0.2f;
            }
            else
            {
                awayTeamPoints += (awayTeam.ShotsForLastX(5) - homeTeam.ShotsForLastX(5))*0.2f;
            }
            // Shots against
            if (homeTeam.HomeShotsAgainstPerGame < awayTeam.AwayShotsAgainstPerGame)
            {
                homeTeamPoints += (awayTeam.AwayShotsAgainstPerGame - homeTeam.HomeShotsAgainstPerGame)*0.2f;
            }
            else
            {
                awayTeamPoints += (homeTeam.HomeShotsAgainstPerGame - awayTeam.AwayShotsAgainstPerGame)*0.2f;
            }
            if (homeTeam.ShotsAgainstPerGame < awayTeam.ShotsAgainstPerGame)
            {
                homeTeamPoints += (awayTeam.ShotsAgainstPerGame - homeTeam.ShotsAgainstPerGame)*0.2f;
            }
            else
            {
                awayTeamPoints += (homeTeam.ShotsAgainstPerGame - awayTeam.ShotsAgainstPerGame)*0.2f;
            }
            if (homeTeam.ShotsAgainstLastX(5) < awayTeam.ShotsAgainstLastX(5))
            {
                homeTeamPoints += (awayTeam.ShotsAgainstLastX(5) - homeTeam.ShotsAgainstLastX(5))*0.2f;
            }
            else
            {
                awayTeamPoints += (homeTeam.ShotsAgainstLastX(5) - awayTeam.ShotsAgainstLastX(5))*0.2f;
            }
            // PIM
            if (homeTeam.HomePIMPerGame < awayTeam.AwayPIMPerGame)
            {
                homeTeamPoints += (awayTeam.AwayPIMPerGame - homeTeam.HomePIMPerGame)*homeTeam.HomePowerPlayPercent;
            }
            else
            {
                awayTeamPoints += (homeTeam.HomePIMPerGame - awayTeam.AwayPIMPerGame)*awayTeam.AwayPowerPlayPercent;
            }
            if (homeTeam.PIMPerGame < awayTeam.PIMPerGame)
            {
                homeTeamPoints += (awayTeam.PIMPerGame - homeTeam.PIMPerGame)*homeTeam.HomePowerPlayPercent;
            }
            else
            {
                awayTeamPoints += (homeTeam.PIMPerGame - awayTeam.PIMPerGame)*awayTeam.AwayPowerPlayPercent;
            }
            if (homeTeam.PIMLastX(5) < awayTeam.PIMLastX(5))
            {
                homeTeamPoints += (awayTeam.PIMLastX(5) - homeTeam.PIMLastX(5))*homeTeam.HomePowerPlayPercent;
            }
            else
            {
                awayTeamPoints += (homeTeam.PIMLastX(5) - awayTeam.PIMLastX(5))*awayTeam.AwayPowerPlayPercent;
            }
            // Win Rate
            if (homeTeam.HomeWinRate > awayTeam.AwayWinRate)
            {
                homeTeamPoints += (homeTeam.HomeWinRate - awayTeam.AwayWinRate)*5f;
            }
            else
            {
                awayTeamPoints += (awayTeam.AwayWinRate - homeTeam.HomeWinRate)*5f;
            }
            if (homeTeam.WinRate > awayTeam.WinRate)
            {
                homeTeamPoints += (homeTeam.WinRate - awayTeam.WinRate)*5f;
            }
            else
            {
                awayTeamPoints += (awayTeam.WinRate - homeTeam.WinRate)*5f;
            }
            if (homeTeam.WinRateLastX(5) > awayTeam.WinRateLastX(5))
            {
                homeTeamPoints += (homeTeam.WinRateLastX(5) - awayTeam.WinRateLastX(5))*5f;
            }
            else
            {
                awayTeamPoints += (awayTeam.WinRateLastX(5) - homeTeam.WinRateLastX(5))*5f;
            }
            // Corsi For
            if (homeTeam.HomeCorsiFor > awayTeam.AwayCorsiFor)
            {
                homeTeamPoints += (homeTeam.HomeCorsiFor - awayTeam.AwayCorsiFor)*0.1f;
            }
            else
            {
                awayTeamPoints += (awayTeam.AwayCorsiFor - homeTeam.HomeCorsiFor)*0.1f;
            }
            if (homeTeam.CorsiFor > awayTeam.CorsiFor)
            {
                homeTeamPoints += (homeTeam.CorsiFor - awayTeam.CorsiFor)*0.1f;
            }
            else
            {
                awayTeamPoints += (awayTeam.CorsiFor - homeTeam.CorsiFor)*0.1f;
            }
            if (homeTeam.CorsiLastX(5) > awayTeam.CorsiLastX(5))
            {
                homeTeamPoints += (homeTeam.CorsiLastX(5) - awayTeam.CorsiLastX(5))*0.1f;
            }
            else
            {
                awayTeamPoints += (awayTeam.CorsiLastX(5) - homeTeam.CorsiLastX(5))*0.1f;
            }
            // Even Strength Goals
            if (homeTeam.HomeEvenStrengthGoalPercent > awayTeam.AwayEvenStrengthGoalPercent)
            {
                homeTeamPoints += homeTeam.HomeEvenStrengthGoalPercent - awayTeam.AwayEvenStrengthGoalPercent;
            }
            else
            {
                awayTeamPoints += awayTeam.AwayEvenStrengthGoalPercent - homeTeam.HomeEvenStrengthGoalPercent;
            }
            if (homeTeam.EvenStrengthGoalPercent > awayTeam.EvenStrengthGoalPercent)
            {
                homeTeamPoints += homeTeam.EvenStrengthGoalPercent - awayTeam.EvenStrengthGoalPercent;
            }
            else
            {
                awayTeamPoints += awayTeam.EvenStrengthGoalPercent - homeTeam.EvenStrengthGoalPercent;
            }

            return homeTeam.Name + ": " + Math.Round(homeTeamPoints,2) + "    |   " + awayTeam.Name + ": " + Math.Round(awayTeamPoints,2);

        }

        #endregion

        #endregion
    }
}
