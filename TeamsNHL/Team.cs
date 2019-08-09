using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using TeamsNHL;

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
        internal List<Skater> skaters;

        public string Name { get => name;}
        public string Abbreviation { get => abbreviation; }
        public List<Skater> Skaters { get => skaters; }

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
            EvenStrengthGoalPercent = 1 - (ppgs.Sum() / goalsFor.Sum());
            PowerPlayPercent = ppgs.Sum() / ppOpportunities.Sum();
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
                HomeEvenStrengthGoalPercent = 1 - (ppgs.Sum() / goalsFor.Sum());
                HomePowerPlayPercent = ppgs.Sum() / ppOpportunities.Sum();
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
                AwayEvenStrengthGoalPercent = 1 - (ppgs.Sum() / goalsFor.Sum());
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

        #region Save/Load

        /// <summary>
        /// Updates AllTeams to the current NHL stats, and writes them to file
        /// </summary>
        internal static void SaveTeams(Team[] teams)
        {
            Stream stream = File.Open(NHL.TEAM_FILE, false ? FileMode.Append : FileMode.Create);
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, teams);
            stream.Close();
        }

        /// <summary>
        /// Returns saved teams
        /// </summary>
        /// <returns></returns>
        internal static Team[] LoadTeams()
        {
            Stream stream = File.Open(NHL.TEAM_FILE, FileMode.Open);
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            Team[] allTeams = (Team[])binaryFormatter.Deserialize(stream);
            stream.Close();
            return allTeams;
            
        }

        #endregion

        #region All Teams

        /// <summary>
        /// Returns the list of all NHL teams.
        /// </summary>
        internal static Team[] AllTeams()
        {
            Team[] allTeams = new Team[NHL.NUMBER_OF_TEAMS];
            string homepage = DownloadString("https://www.hockey-reference.com/");
            string teamTableData = homepage.Split(new string[] { "selector_0" }, StringSplitOptions.None)[1];
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

        #endregion

        #region Write File Methods

        /// <summary>
        /// Returns a string formatted to HTML with comparison stats between home and away teams
        /// </summary>
        /// <param name="homeTeam"></param>
        /// <param name="awayTeam"></param>
        /// <returns></returns>
        internal static string WriteMatchupHTML(Team homeTeam, Team awayTeam)
        {
            string homeHTML = "";
            string awayHTML = "";
            int homeScore = 0;
            int awayScore = 0;
            int lastX = 5;
            // Headers
            string header = $"<br /><h4>{homeTeam.Name} vs. {awayTeam.Name}</h4>\n";
            homeHTML += $"<div style=\"float:left;margin-right:4.5em;\"><br /><h3>{homeTeam.Name}</h3>\n<h2>Home Averages</h2>";
            awayHTML += $"<div style=\"display:block\"><br /><h3>{awayTeam.Name}</h3>\n<h2>Away Averages</h2>";
            // Arena Win Rate
            string[] winRateArena = StatHTMLColoured("Win Rate", homeTeam.HomeWinRate, awayTeam.AwayWinRate, true);

            homeHTML += winRateArena[0];
            awayHTML += winRateArena[1];
            homeScore += Convert.ToInt32(winRateArena[2]);
            awayScore += Convert.ToInt32(winRateArena[3]);
            // Goals For
            string[] goalsForArena = StatHTMLColoured("Goals For", homeTeam.HomeGoalsForPerGame, awayTeam.AwayGoalsForPerGame, true);
            homeHTML += goalsForArena[0];
            awayHTML += goalsForArena[1];
            homeScore += Convert.ToInt32(goalsForArena[2]);
            awayScore += Convert.ToInt32(goalsForArena[3]);
            // Goals Against
            string[] goalsAgainstArena = StatHTMLColoured("Goals Against", homeTeam.HomeGoalsAgainstPerGame, awayTeam.AwayGoalsAgainstPerGame, false);
            homeHTML += goalsAgainstArena[0];
            awayHTML += goalsAgainstArena[1];
            homeScore += Convert.ToInt32(goalsAgainstArena[2]);
            awayScore += Convert.ToInt32(goalsAgainstArena[3]);
            // Shots For
            string[] shotsForArena = StatHTMLColoured("Shots For", homeTeam.HomeShotsForPerGame, awayTeam.AwayShotsForPerGame, true);
            homeHTML += shotsForArena[0];
            awayHTML += shotsForArena[1];
            homeScore += Convert.ToInt32(shotsForArena[2]);
            awayScore += Convert.ToInt32(shotsForArena[3]);
            // Shots Against
            string[] shotsAgainstArena = StatHTMLColoured("Shots Against", homeTeam.HomeShotsAgainstPerGame, awayTeam.AwayShotsAgainstPerGame, false);
            homeHTML += shotsAgainstArena[0];
            awayHTML += shotsAgainstArena[1];
            homeScore += Convert.ToInt32(shotsAgainstArena[2]);
            awayScore += Convert.ToInt32(shotsAgainstArena[3]);
            // PIM
            string[] PIMArena = StatHTMLColoured("Penalty Minutes", homeTeam.HomePIMPerGame, awayTeam.AwayPIMPerGame, false);
            homeHTML += PIMArena[0];
            awayHTML += PIMArena[1];
            homeScore += Convert.ToInt32(PIMArena[2]);
            awayScore += Convert.ToInt32(PIMArena[3]);
            // Last 5 game averages
            homeHTML += "<h2>Last 5 Games</h2>";
            awayHTML += "<h2>Last 5 Games</h2>";
            // Goals For
            string[] goalsForX = StatHTMLColoured("Goals For", homeTeam.GoalsForLastX(lastX), awayTeam.GoalsForLastX(lastX), true);
            homeHTML += goalsForX[0];
            awayHTML += goalsForX[1];
            homeScore += Convert.ToInt32(goalsForX[2]);
            awayScore += Convert.ToInt32(goalsForX[3]);
            // Goals Against
            string[] goalsAgainstX = StatHTMLColoured("Goals Against", homeTeam.GoalsAgainstLastX(lastX), awayTeam.GoalsAgainstLastX(lastX), false);
            homeHTML += goalsAgainstX[0];
            awayHTML += goalsAgainstX[1];
            homeScore += Convert.ToInt32(goalsAgainstX[2]);
            awayScore += Convert.ToInt32(goalsAgainstX[3]);
            // Shots For
            string[] shotsForX = StatHTMLColoured("Shots For", homeTeam.ShotsForLastX(lastX), awayTeam.ShotsForLastX(lastX), true);
            homeHTML += shotsForX[0];
            awayHTML += shotsForX[1];
            homeScore += Convert.ToInt32(shotsForX[2]);
            awayScore += Convert.ToInt32(shotsForX[3]);
            // Shots Against
            string[] shotsAgainstX = StatHTMLColoured("Shots Against", homeTeam.ShotsAgainstLastX(lastX), awayTeam.ShotsAgainstLastX(lastX), false);
            homeHTML += shotsAgainstX[0];
            awayHTML += shotsAgainstX[1];
            homeScore += Convert.ToInt32(shotsAgainstX[2]);
            awayScore += Convert.ToInt32(shotsAgainstX[3]);
            // PIM
            string[] PIMX = StatHTMLColoured("Penalty Minutes", homeTeam.PIMLastX(lastX), awayTeam.PIMLastX(lastX), false);
            homeHTML += PIMX[0];
            awayHTML += PIMX[1];
            homeScore += Convert.ToInt32(PIMX[2]);
            awayScore += Convert.ToInt32(PIMX[3]);
            // Advanced Stats
            homeHTML += "<h2>Advanced Stats</h2>";
            awayHTML += "<h2>Advanced Stats</h2>";
            // Corsi For
            string[] corsiForArena = StatHTMLColoured("Corsi For", homeTeam.HomeCorsiFor, awayTeam.AwayCorsiFor, true);
            homeHTML += corsiForArena[0];
            awayHTML += corsiForArena[1];
            homeScore += Convert.ToInt32(corsiForArena[2]);
            awayScore += Convert.ToInt32(corsiForArena[3]);
            // Even Strength Goal Percentage
            string[] evenGoals = StatHTMLColoured("Even Strength Goal %", homeTeam.HomeEvenStrengthGoalPercent*100, awayTeam.AwayEvenStrengthGoalPercent*100, true);
            homeHTML += evenGoals[0];
            awayHTML += evenGoals[1];
            homeScore += Convert.ToInt32(evenGoals[2]);
            awayScore += Convert.ToInt32(evenGoals[3]);
            // Totals
            string[] totals = StatHTMLColoured("Categories Won", homeScore, awayScore, true);
            homeHTML += totals[0];
            awayHTML += totals[1];


            return header  + homeHTML + "</div>\n"+ awayHTML + "</div>\n";
        }

        /// <summary>
        /// Returns an array of string where index 0 is the home team HTML and index 1 is the away team HTML
        /// </summary>
        /// <param name="title"></param>
        /// <param name="homeStat"></param>
        /// <param name="awayStat"></param>
        /// <param name="isHighBetter"></param>
        /// <returns></returns>
        private static string[] StatHTMLColoured(string title, float homeStat, float awayStat, bool isHighBetter)
        {
            string homeColour = "";
            string awayColour = "";
            int homeScore = 0;
            int awayScore = 0;
            string[] htmlStrings = new string[4];
            if (isHighBetter)
            {
                if (homeStat > awayStat)
                {
                    homeColour = "green";
                    awayColour = "red";
                    homeScore++;
                }
                else if (homeStat == awayStat)
                {
                    homeColour = "blue";
                    awayColour = "blue";
                }
                else
                {
                    homeColour = "red";
                    awayColour = "green";
                    awayScore++;
                }
            } else
            {
                if (homeStat < awayStat)
                {
                    homeColour = "green";
                    awayColour = "red";
                    homeScore++;
                }
                else if (homeStat == awayStat)
                {
                    homeColour = "blue";
                    awayColour = "blue";
                }
                else
                {
                    homeColour = "red";
                    awayColour = "green";
                    awayScore++;
                }
            }
            htmlStrings[0] = StatLine(title, homeColour, homeStat);
            htmlStrings[1] = StatLine(title, awayColour, awayStat);
            htmlStrings[2] = homeScore.ToString();
            htmlStrings[3] = awayScore.ToString();
            return htmlStrings;
        }

        /// <summary>
        /// Returns a string formatted to HTML
        /// </summary>
        /// <param name="title"></param>
        /// <param name="colour"></param>
        /// <param name="stat"></param>
        /// <returns></returns>
        private static string StatLine(string title, string colour, float stat)
        {
            if (title == "Categories Won")
            {
                return $"<br /><b>{title}: <b style=\"color:{colour};font-size:1.2em;\">{Math.Round(stat, 2).ToString()}</b></b>\n";
            }

            return $"<p>{title}: <b style=\"color:{colour};font-size:1.2em;\">{Math.Round(stat, 2).ToString()}</b></p>\n";
        }

        #endregion

        #endregion
    }
}
