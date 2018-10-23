using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/* Nathan Saccon NHLTeamStats Project
 *          Date Started: October 21, 2018: Created project from scratch
 * 
 */

namespace NHLTeamStats
{
    [Serializable]
    class NHLTeams
    {
        #region Global Variables

        private const int NUMBER_OF_TEAMS = 31;
        private const int GAMES_PER_SEASON = 82;
        private const int YEAR = 2019; // eg 2019 gets stats for the 2018/2019 season.
        private const string HOMEPAGE = "https://www.hockey-reference.com/";

        private string Homepage = DownloadString(HOMEPAGE);
        private string filePath = "NHLTeams.txt";

        private NHLTeam[] allTeams = new NHLTeam[NUMBER_OF_TEAMS];

        public NHLTeam[] AllTeams { get => allTeams; set => allTeams = value; }

        #endregion

        #region Constructor
        /// <summary>
        /// Tries to load all teams from a file, uses UpdateAllTeams() otherwise.
        /// </summary>
        public NHLTeams()
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    AllTeams = (NHLTeam[])binaryFormatter.Deserialize(stream);
                }
            }
            catch (Exception)
            {
                UpdateAllTeams();
            }

        }

        #endregion

        #region Webclient Method

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

        #region Update Team Stats

        /// <summary>
        /// Updates AllTeams to the current NHL stats, and writes them to file
        /// </summary>
        public void UpdateAllTeams()
        {
            PopulateNamesAndAbbreviations();
            PopulateGamelogs(YEAR);

            using (Stream stream = File.Open(filePath, false ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, AllTeams);
            }
        }

        #region Name and Abbreviation Methods

        /// <summary>
        /// Adds new nhl teams to AllTeams with their name and abbreviation set
        /// </summary>
        private void PopulateNamesAndAbbreviations()
        {
            string teamTableData = Homepage.Split(new string[] { "selector_0" }, StringSplitOptions.None)[1];
            int initialRow = 2;
            for (int i = initialRow; i < initialRow + NUMBER_OF_TEAMS; i++)
            {

                string rowData = teamTableData.Split(new string[] { "<option value=\"" }, StringSplitOptions.None)[i];
                string currentAbbreviation = rowData.Split(new string[] { "\"" }, StringSplitOptions.None)[0];
                string currentName = rowData.Split('>')[1];
                currentName = currentName.Split('<')[0];

                NHLTeam newTeam = new NHLTeam();
                newTeam.Name = currentName;
                newTeam.Abbreviation = currentAbbreviation;
                AllTeams[i - initialRow] = newTeam;
            }
        }

        #endregion

        #region Gamelog Methods

        /// <summary>
        /// Adds the year's Gamelog to each team in AllTeams using their abbreviation
        /// </summary>
        /// <param name="year"></param>
        private void PopulateGamelogs(int year)
        {
            for (int i = 0; i < NUMBER_OF_TEAMS; i++)
            {
                AllTeams[i].Gamelog = TeamStatPageToGameList(AllTeams[i].Abbreviation, year);
            }
        }

        /// <summary>
        /// Returns a list of all the games a team (by abbreviation) played in a year.
        /// </summary>
        /// <param name="abbreviation"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        private List<Game> TeamStatPageToGameList(string abbreviation, int year)
        {
            string teamStatURL = HOMEPAGE + "teams/" + abbreviation + "/" + year.ToString() + "_gamelog.html";
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

        #endregion
    }
}
