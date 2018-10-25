using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

/* Nathan Saccon TeamStats Project
 *          Date Started: October 21, 2018: Created project from scratch
 * 
 */

namespace NHLTeamsLib
{
    [Serializable]
    public class NHL
    {
        #region Global Variables
        
        private const string FILE = "NHLTeams.txt";

        internal const int NUMBER_OF_TEAMS = 31;
        internal const int GAMES_PER_SEASON = 82;
        internal const int YEAR = 2019; // eg 2019 gets stats for the 2018/2019 season.
        internal const string HOMEPAGE = "https://www.hockey-reference.com/";
        internal string Homepage = Team.DownloadString(HOMEPAGE);

        private Team[] allTeams = new Team[NUMBER_OF_TEAMS];

        public Team[] AllTeams { get => allTeams; }

        #endregion

        #region Constructor

        /// <summary>
        /// Tries to load all teams from a file, uses the internet otherwise.
        /// </summary>
        public NHL(bool load)
        {
            if (load)
            {
                try
                {
                    using (Stream stream = File.Open(FILE, FileMode.Open))
                    {
                        var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                        allTeams = (Team[])binaryFormatter.Deserialize(stream);
                    }
                }
                catch (Exception)
                {
                    WriteNewTeamsFile();
                }
            }
            else
            {
                WriteNewTeamsFile();
            }

        }

        /// <summary>
        /// Updates AllTeams to the current NHL stats, and writes them to file
        /// </summary>
        private void WriteNewTeamsFile()
        {
            allTeams = Team.AllTeams();

            using (Stream stream = File.Open(FILE, false ? FileMode.Append : FileMode.Create))
            {
                var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(stream, AllTeams);
            }
        }

        #endregion

        #region Matchup Methods

        /// <summary>
        /// Returns a list of today's matchups
        /// </summary>
        public List<Team[]> Matchups()
        {
            DateTime date = DateTime.Now;
            string dateAsStr = date.Year + "-" + date.Month + "-" + date.Day;
            return Team.MatchupsByDate(AllTeams, dateAsStr);
        }

        /// <summary>
        /// Returns a list of matchups on a given date
        /// </summary>
        public List<Team[]> Matchups(int daysAhead)
        {
            DateTime date = DateTime.Now.AddDays(1);
            string dateStr = date.Year + "-" + date.Month + "-" + date.Day;
            return Team.MatchupsByDate(AllTeams, dateStr);
        }

        /// <summary>
        /// Returns a list of matchups on a given date
        /// </summary>
        public List<Team[]> Matchups(string dateStr)
        {
            return Team.MatchupsByDate(AllTeams, dateStr);
        }

        #endregion
    }
}
