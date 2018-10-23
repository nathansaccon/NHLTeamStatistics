﻿using System;
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
        private string filePath = "NHLTeams.txt";

        private NHLTeam[] allTeams = new NHLTeam[NUMBER_OF_TEAMS];

        public NHLTeam[] AllTeams { get => allTeams; set => allTeams = value; }

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
                    AllTeams = (NHLTeam[])binaryFormatter.Deserialize(stream);
                }
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
    }
}