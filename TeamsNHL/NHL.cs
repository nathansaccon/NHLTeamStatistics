using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        internal const string DRAFTKINGS_FILE = "DKSalaries.csv";
        internal const string FANDUEL_FILE = "FDSalaries.csv";
        internal const int NUMBER_OF_TEAMS = 31;
        internal const int GAMES_PER_SEASON = 82;
        internal const int YEAR = 2019; // eg 2019 gets stats for the 2018/2019 season.
        internal const bool TEST = false;

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

            if (loadTeams)
            {
                try
                {
                    allTeams = Team.LoadTeams();
                }
                catch (Exception)
                {


                    Team[] teams = Team.AllTeams();
                    for (int i = 0; i < teams.Length; i++)
                    {
                        teams[i].skaters = allSkaters.Where(s => s.TeamAbbreviation == teams[i].Abbreviation).ToList();
                    }
                    Team.SaveTeams(teams);
                    allTeams = Team.LoadTeams();
                }
            }
            else
            {
                Team[] teams = Team.AllTeams();
                for (int i = 0; i < teams.Length; i++)
                {
                    teams[i].skaters = allSkaters.Where(s => s.TeamAbbreviation == teams[i].Abbreviation).ToList();
                }
                Team.SaveTeams(teams);
                allTeams = Team.LoadTeams();
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
            string day = date.Day.ToString();
            string month = date.Month.ToString();
            if(date.Day - 10 < 1)
            {
                day = "0" + day;
            }
            if (date.Month - 10 < 1)
            {
                month = "0" + month;
            }
            string dateAsStr = date.Year + "-" + month + "-" + day;
            return Team.MatchupsByDate(AllTeams, dateAsStr);
        }

        /// <summary>
        /// Returns a list of matchups on a given date
        /// </summary>
        public List<Team[]> Matchups(int daysAhead)
        {
            DateTime date = DateTime.Now.AddDays(daysAhead);
            string day = date.Day.ToString();
            string month = date.Month.ToString();
            if (date.Day - 10 < 0)
            {
                day = "0" + day;
            }
            if (date.Month - 10 < 1)
            {
                month = "0" + month;
            }
            string dateStr = date.Year + "-" + month + "-" + day;
            return Team.MatchupsByDate(AllTeams, dateStr);
        }

        /// <summary>
        /// Returns a list of matchups on a given date
        /// </summary>
        public List<Team[]> Matchups(string dateStr)
        {
            return Team.MatchupsByDate(AllTeams, dateStr);
        }

        /// <summary>
        /// Returns a list of strings that contain the team abbriviations that play on given day
        /// </summary>
        /// <param name="daysAhead"></param>
        /// <returns></returns>
        public List<string> MatchupAllTeams(int daysAhead = 0)
        {
            List<string> allTeams = new List<string>();
            foreach(Team[] teams in Matchups(daysAhead))
            {
                allTeams.Add(teams[0].Abbreviation);
                allTeams.Add(teams[1].Abbreviation);
            }
            return allTeams;
        }

        #endregion

        #region HTML Methods
        /// <summary>
        /// Writes a file that contains HTML code to format matchups.
        /// </summary>
        /// <param name="daysAhead"></param>
        public void WriteMatchupHTML(int daysAhead = 0)
        {
            StreamWriter writer = new StreamWriter("HTMLStats.txt");
            DateTime date = DateTime.Now.AddDays(daysAhead);
            string header = $"<div style=\"font-family:verdana;\">NHL Team Matchup Stats - {date.ToString("MMMM")} {date.Day}, {date.Year}";
            string footer = "</div>";
            string output = "";
            foreach(Team[] team in Matchups(daysAhead))
            {
                output += Team.WriteMatchupHTML(team[0], team[1]);
            }
            writer.Write(header + output + footer);
            writer.Close();
        }

        /// <summary>
        /// Writes a file that contains HTML code for value players
        /// </summary>
        /// <param name="daysAhead"></param>
        public void WriteDraftKingsValueHTML(int daysAhead = 0)
        {
            StreamWriter writer = new StreamWriter("HTMLDraftKings.txt");
            DateTime date = DateTime.Now.AddDays(daysAhead);
            string header = $"<div style=\"font-family:verdana;\">DraftKings Value Players - {date.ToString("MMMM")} {date.Day}, {date.Year}";
            string footer = "</div>";
            string output = "";
            int minCost = 3800;
            int maxCost = 6000;
            foreach (Skater skater in DraftKingsValuePlayers(maxCost, minCost, "C", 3, daysAhead))
            {
                output += skater.ToDraftKingsHTML();
            }
            foreach (Skater skater in DraftKingsValuePlayers(maxCost, minCost, "W", 3, daysAhead))
            {
                output += skater.ToDraftKingsHTML();
            }
            foreach (Skater skater in DraftKingsValuePlayers(maxCost, minCost, "D", 3, daysAhead))
            {
                output += skater.ToDraftKingsHTML();
            }
            writer.Write(header + output + footer);
            writer.Close();
        }

        /// <summary>
        /// Writes a file that contains HTML code for value players
        /// </summary>
        /// <param name="daysAhead"></param>
        public void WriteFanDuelValueHTML(int daysAhead = 0)
        {
            StreamWriter writer = new StreamWriter("HTMLFanDuel.txt");
            DateTime date = DateTime.Now.AddDays(daysAhead);
            string header = $"<div style=\"font-family:verdana;\">FanDuel Value Players - {date.ToString("MMMM")} {date.Day}, {date.Year}";
            string footer = "</div>";
            string output = "";
            int minCost = 4000;
            int maxCost = 8800;
            foreach (Skater skater in FanDuelValuePlayers(maxCost, minCost, "C", 3, daysAhead))
            {
                output += skater.ToFanDuelHTML();
            }
            foreach (Skater skater in FanDuelValuePlayers(maxCost, minCost, "W", 3, daysAhead))
            {
                output += skater.ToFanDuelHTML();
            }
            foreach (Skater skater in FanDuelValuePlayers(maxCost, minCost, "D", 3, daysAhead))
            {
                output += skater.ToFanDuelHTML();
            }
            writer.Write(header + output + footer);
            writer.Close();
        }

        #endregion

        #region Fantasy Hockey Methods

        /// <summary>
        /// Returns the sorted skater list by DraftKings average
        /// </summary>
        /// <returns></returns>
        public List<Skater> DraftKingsValuePlayers(int maxCost, int minCost, string position, int lengthOfReturned, int daysAhead)
        {
            List<string> teamsPlaying = MatchupAllTeams(daysAhead);
            List<Skater> ordered = allSkaters.Where(s => teamsPlaying.Contains(s.TeamAbbreviation)).ToList().OrderBy(x => -x.DraftKingsValue()).ToList();
            List<Skater> topSkaters = new List<Skater>();
            
            int players = 0;

            for (int i = 0; i < ordered.Count; i++)
            {
                int cost = ordered[i].draftKingsCost;
                string pos = ordered[i].draftKingsPosition;
                if (pos == "LW" || pos == "RW")
                {
                    pos = "W";
                }

                if (!ordered[i].isInjured && cost > minCost && cost < maxCost && pos == position)
                {
                    topSkaters.Add(ordered[i]);
                    players++;
                    if (players == lengthOfReturned)
                    {
                        break;
                    }
                }
            }
            return topSkaters;
        }

        /// <summary>
        /// Returns the sorted skater list by DraftKings average
        /// </summary>
        /// <returns></returns>
        public List<Skater> FanDuelValuePlayers(int maxCost, int minCost, string position, int lengthOfReturned, int daysAhead)
        {
            List<string> teamsPlaying = MatchupAllTeams(daysAhead);
            List<Skater> ordered = allSkaters.Where(s => teamsPlaying.Contains(s.TeamAbbreviation)).ToList().OrderBy(x => -x.FanDuelValue()).ToList();

            List<Skater> topSkaters = new List<Skater>();
            int players = 0;

            for (int i = 0; i < ordered.Count; i++)
            {
                int cost = ordered[i].fanDuelCost;
                string pos = ordered[i].fanDuelPosition;
                if (pos == "LW" || pos == "RW")
                {
                    pos = "W";
                }
                bool playing = teamsPlaying.Contains(ordered[i].TeamAbbreviation);

                if (playing && !ordered[i].isInjured && cost > minCost && cost < maxCost && pos == position)
                {
                    topSkaters.Add(ordered[i]);
                    players++;
                    if (players == lengthOfReturned)
                    {
                        break;
                    }
                }
            }
            return topSkaters;
        }

        #endregion
    }
}
