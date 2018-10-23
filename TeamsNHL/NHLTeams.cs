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

namespace NHLTeamsLib
{
    [Serializable]
    public class NHLTeams
    {
        #region Global Variables

        private const int NUMBER_OF_TEAMS = 31;
        private const int GAMES_PER_SEASON = 82;
        private const int YEAR = 2019; // eg 2019 gets stats for the 2018/2019 season.
        private const string HOMEPAGE = "https://www.hockey-reference.com/";

        private string Homepage = DownloadString(HOMEPAGE);
        private const string filePath = "NHLTeams.txt";

        private NHLTeam[] allTeams = new NHLTeam[NUMBER_OF_TEAMS];
        private NHLTeam[,] matchupList = new NHLTeam[NUMBER_OF_TEAMS, 2];

        public NHLTeam[] AllTeams { get => allTeams; }

        #endregion

        #region Constructor

        /// <summary>
        /// Tries to load all teams from a file, uses the internet otherwise.
        /// </summary>
        public NHLTeams()
        {
            try
            {
                using (Stream stream = File.Open(filePath, FileMode.Open))
                {
                    var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    allTeams = (NHLTeam[])binaryFormatter.Deserialize(stream);
                }
                PopulateMatchups();
            }
            catch (Exception)
            {
                UpdateAllTeams();
            }

        }

        /// <summary>
        /// Uses internet to generate a file with today's team stats.
        /// </summary>
        public NHLTeams(bool firstLoad)
        {
            UpdateAllTeams();
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
        private void UpdateAllTeams()
        {
            PopulateNamesAndAbbreviations();
            PopulateTeamData(YEAR);
            PopulateMatchups();

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

        #region Populate Teams

        /// <summary>
        /// Adds the year's Gamelog to each team in AllTeams using their abbreviation
        /// </summary>
        /// <param name="year"></param>
        private void PopulateTeamData(int year)
        {
            foreach(NHLTeam team in AllTeams)
            {
                team.PopulateTeam(year);
            }
        }
        #endregion

        #endregion

        #region Populate Matchups

        /// <summary>
        /// Populates the MatchupList with team[0] being the chosen team, and team[1] being their next opponent.
        /// </summary>
        private void PopulateMatchups()
        {
            for (int i = 0; i < NUMBER_OF_TEAMS; i++)
            {
                NHLTeam currentTeam = AllTeams[i];
                matchupList[i, 0] = currentTeam;
                matchupList[i, 1] = NHLTeam.NextOpponent(currentTeam, AllTeams);
            }
        }


        #endregion

        #region Matchup Methods

        /// <summary>
        /// Returns a list of pairs of teams that play against each other today
        /// </summary>
        /// <returns></returns>
        public List<NHLTeam[]> TodaysMatchups()
        {
            List<NHLTeam[]> matchupPairs = new List<NHLTeam[]>();
            List<string> teamsAdded = new List<string>();
            for (int i = 0; i < matchupList.GetLength(0); i++)
            {
                string matchupDate = matchupList[i, 0].NextGameDate;
                string todaysDate = DateTime.Now.Year.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Day.ToString();
                if (matchupDate == todaysDate && !teamsAdded.Contains(matchupList[i, 0].Name))
                {
                    NHLTeam[] pairing = new NHLTeam[2];
                    pairing[0] = matchupList[i, 0];
                    pairing[1] = matchupList[i, 1];
                    teamsAdded.Add(matchupList[i, 0].Name);
                    teamsAdded.Add(matchupList[i, 1].Name);
                    matchupPairs.Add(pairing);
                }
            }
            return matchupPairs;
        }

        #endregion

    }
}
