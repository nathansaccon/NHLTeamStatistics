using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TeamsNHL;

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
        
        internal const string TEAM_FILE = "NHLTeams.txt";
        internal const string SKATER_FILE = "NHLSkaters.txt";
        internal const int NUMBER_OF_TEAMS = 31;
        internal const int GAMES_PER_SEASON = 82;
        internal const int YEAR = 2019; // eg 2019 gets stats for the 2018/2019 season.

        private Team[] allTeams = new Team[NUMBER_OF_TEAMS];
        private List<Skater> allSkaters = new List<Skater>();

        public Team[] AllTeams { get => allTeams; }
        public List<Skater> AllSkaters { get => allSkaters; }

        #endregion

        #region Constructor

        /// <summary>
        /// Tries to load all teams from a file, uses the internet otherwise.
        /// </summary>
        public NHL(bool loadTeams, bool loadSkaters)
        {
            if (loadTeams)
            {
                try
                {
                    allTeams = Team.LoadTeams();
                }
                catch (Exception)
                {
                    Team.SaveTeams(Team.AllTeams());
                    allTeams = Team.LoadTeams();
                }
            }
            else
            {
                Team.SaveTeams(Team.AllTeams());
                allTeams = Team.LoadTeams();
            }

            if (loadSkaters)
            {
                try
                {
                    allSkaters = Skater.LoadSkaters();
                }
                catch (Exception)
                {
                    Skater.SaveSkaters(Skater.AllSkaters());
                    allSkaters = Skater.LoadSkaters();
                }
            }
            else
            {
                Skater.SaveSkaters(Skater.AllSkaters());
                allSkaters = Skater.LoadSkaters();
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
