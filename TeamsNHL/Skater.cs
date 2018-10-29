using NHLTeamsLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamsNHL
{
    [Serializable]
    public class Skater : Player
    {
        #region Constructor

        public Skater(string url)
        {
            this.Url = url;
            this.PopulateSkater();
        }

        #endregion

        #region Statistic Variables

        public float Goals;
        public float HomeGoals;
        public float AwayGoals;

        public float Assists;
        public float HomeAssists;
        public float AwayAssists;

        public float PlusMinus;
        public float HomePlusMinus;
        public float AwayPlusMinus;

        public float PIM;
        public float HomePIM;
        public float AwayPIM;

        public float Shots;
        public float HomeShots;
        public float AwayShots;

        public float TimeOnIce;
        public float HomeTimeOnIce;
        public float AwayTimeOnIce;

        public float Hits;
        public float HomeHits;
        public float AwayHits;

        public float Blocks;
        public float HomeBlocks;
        public float AwayBlocks;

        #endregion

        #region Populate Methods

        /// <summary>
        /// Populates the skater with al their information
        /// </summary>
        private void PopulateSkater()
        {
            PopulateNameAndGamelog();
            PopulateSkaterStats();
        }

        #region Populate Name and Gamelog

        /// <summary>
        /// Populates the player with name, position, and gamelog
        /// </summary>
        private void PopulateNameAndGamelog()
        {
            string teamStatURL = "https://www.hockey-reference.com/players/"+ Url[0] + "/" + Url + "/gamelog/"+NHL.YEAR.ToString();
            string page = Team.DownloadString(teamStatURL);
            string headerData = page.Split(new string[] { "<h1 itemprop=\"name\">" }, StringSplitOptions.None)[1];

            this.name = headerData.Split('<')[0];
            this.position = headerData.Split(new string[] { "Position</strong>: " }, StringSplitOptions.None)[1].Split('&')[0];

            string[] pageData = page.Split(new string[] { "<tr id=\"gamelog." }, StringSplitOptions.None);

            List<Game> allGamesPlayed = new List<Game>();
            int index = 1;
            bool stopper = true;
            while (stopper)
            {
                string columnData = pageData[index];
                if (columnData.Contains("</table>"))
                {
                    stopper = false;
                }
                string[] columns = columnData.Split('>');
                Game game = new Game();
                // Date
                game.Date = columns[5].Split('<')[0];
                // Team
                if(index == 1)
                {
                    teamAbbreviation = columns[13].Split('<')[0];
                }
                // Arena
                if (columns[16].Split('<')[0] == "@")
                {
                    game.Arena = Arena.AWAY;
                }
                else
                {
                    game.Arena = Arena.HOME;
                }
                // Opponent
                game.Opponent = columns[19].Split('<')[0];
                // Goals
                game.GoalsFor = Convert.ToInt32(rowToStat(columns[24]));
                // Assists
                game.Assists = Convert.ToInt32(rowToStat(columns[26]));
                // Plus Minus
                game.PlusMinus = Convert.ToInt32(rowToStat(columns[30]));
                // PIM
                game.PenaltyMinutes = Convert.ToInt32(rowToStat(columns[32]));
                // Powerplay Goals
                game.PowerplayGoals = Convert.ToInt32(rowToStat(columns[36]));
                // Shots
                game.ShotsFor = Convert.ToInt32(rowToStat(columns[48]));
                // Time On Ice
                game.TimeOnIce = rowToStat(columns[54]);
                // Hits
                game.Hits = Convert.ToInt32(rowToStat(columns[56]));
                // Blocks
                game.Blocks = Convert.ToInt32(rowToStat(columns[58]));

                allGamesPlayed.Add(game);

                index++;
            }
            gamelog = allGamesPlayed;
        }

        /// <summary>
        /// Returns the first string in a row before the < character
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        private string rowToStat(string row)
        {
            string value = "";
            try
            {
                value = row.Split('<')[0];
            }
            catch (Exception)
            {
                value = "0";
            }
            if (value == "")
            {
                return "0";
            }else
            {
                return value;
            }
        }

        #endregion

        #region Populate Stats

        /// <summary>
        /// Populates skater's stat variables
        /// </summary>
        private void PopulateSkaterStats()
        {
            PopulateSeasonStats();
            PopulateArenaStats(Arena.HOME);
            PopulateArenaStats(Arena.AWAY);
        }


        /// <summary>
        /// Populates the skater season stats
        /// </summary>
        private void PopulateSeasonStats()
        {
            List<float> goals = new List<float>();
            List<float> assists = new List<float>();
            List<float> plusMinus = new List<float>();
            List<float> pim = new List<float>();
            List<float> shots = new List<float>();
            List<float> toi = new List<float>();
            List<float> hits = new List<float>();
            List<float> blocks = new List<float>();

            foreach (Game game in gamelog)
            {
                goals.Add(game.GoalsFor);
                assists.Add(game.Assists);
                plusMinus.Add(game.PlusMinus);
                pim.Add(game.PenaltyMinutes);
                shots.Add(game.ShotsFor);
                toi.Add(TOIToMinutes(game.TimeOnIce));
                hits.Add(game.Hits);
                blocks.Add(game.Blocks);
            }

            Goals = goals.Average();
            Assists = assists.Average();
            PlusMinus = plusMinus.Average();
            PIM = pim.Average();
            Shots = shots.Average();
            TimeOnIce = toi.Average();
            Hits = hits.Average();
            Blocks = blocks.Average();
        }

        /// <summary>
        /// Populates the home/away skater stats
        /// </summary>
        private void PopulateArenaStats(Arena arena)
        {
            List<float> goals = new List<float>();
            List<float> assists = new List<float>();
            List<float> plusMinus = new List<float>();
            List<float> pim = new List<float>();
            List<float> shots = new List<float>();
            List<float> toi = new List<float>();
            List<float> hits = new List<float>();
            List<float> blocks = new List<float>();

            foreach (Game game in gamelog)
            {
                if (game.Arena == arena)
                {
                    goals.Add(game.GoalsFor);
                    assists.Add(game.Assists);
                    plusMinus.Add(game.PlusMinus);
                    pim.Add(game.PenaltyMinutes);
                    shots.Add(game.ShotsFor);
                    toi.Add(TOIToMinutes(game.TimeOnIce));
                    hits.Add(game.Hits);
                    blocks.Add(game.Blocks);
                }
            }

            if(arena == Arena.HOME && goals.Count == 0)
            {
                HomeGoals = 0;
                HomeAssists = 0;
                HomePlusMinus = 0;
                HomePIM = 0;
                HomeShots = 0;
                HomeTimeOnIce = 0;
                HomeHits = 0;
                HomeBlocks = 0;
            } else if(arena == Arena.AWAY && goals.Count == 0)
            {
                AwayGoals = 0;
                AwayAssists = 0;
                AwayPlusMinus = 0;
                AwayPIM = 0;
                AwayShots = 0;
                AwayTimeOnIce = 0;
                AwayHits = 0;
                AwayBlocks = 0;
            } else if (arena == Arena.HOME)
            {
                HomeGoals = goals.Average();
                HomeAssists = assists.Average();
                HomePlusMinus = plusMinus.Average();
                HomePIM = pim.Average();
                HomeShots = shots.Average();
                HomeTimeOnIce = toi.Average();
                HomeHits = hits.Average();
                HomeBlocks = blocks.Average();
            }else
            {
                AwayGoals = goals.Average();
                AwayAssists = assists.Average();
                AwayPlusMinus = plusMinus.Average();
                AwayPIM = pim.Average();
                AwayShots = shots.Average();
                AwayTimeOnIce = toi.Average();
                AwayHits = hits.Average();
                AwayBlocks = blocks.Average();
            }
        }

        #endregion

        #endregion

        #region Math Methods

        /// <summary>
        /// Returns the time in minutes from a TIO stat
        /// </summary>
        /// <param name="TIO"></param>
        /// <returns></returns>
        private float TOIToMinutes(string time)
        {
            float totalTime = 0;
            string[] splitTime = time.Split(':');
            totalTime += Convert.ToInt32(splitTime[0]);
            totalTime += (float) (Convert.ToDecimal(splitTime[1]) / 60);
            return totalTime;
        }

        #endregion

        #region Static Methods

        #region Save, Load

        /// <summary>
        /// Updates AllTeams to the current NHL stats, and writes them to file
        /// </summary>
        internal static void SaveSkaters(List<Skater> skaters)
        {
            Stream stream = File.Open(NHL.SKATER_FILE, false ? FileMode.Append : FileMode.Create);
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            binaryFormatter.Serialize(stream, skaters);
            stream.Close();
        }

        /// <summary>
        /// Returns saved teams
        /// </summary>
        /// <returns></returns>
        internal static List<Skater> LoadSkaters()
        {
            Stream stream = File.Open(NHL.SKATER_FILE, FileMode.Open);
            var binaryFormatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
            List<Skater> allSkaters = (List<Skater>)binaryFormatter.Deserialize(stream);
            stream.Close();
            return allSkaters;

        }

        #endregion

        /// <summary>
        /// Returns a list of all skaters in a given year
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static List<Skater> AllSkaters()
        {
            List<Skater> allSkaters = new List<Skater>();
            List<string> alreadyAdded = new List<string>();
            string homepage = Team.DownloadString("http://www.hockey-reference.com/leagues/NHL_"+NHL.YEAR.ToString()+"_skaters.html");
            int numberOfTeams = Convert.ToInt32(homepage.Split(new string[] { "data-line-count=\"" }, StringSplitOptions.None)[1].Split('"')[0]);
            string[] tableData = homepage.Split(new string[] { "data-append-csv=\"" }, StringSplitOptions.None);

            for (int i = 1; i < numberOfTeams+1; i++)
            {
                string playerURL = tableData[i].Split('"')[0];
                if (!alreadyAdded.Contains(playerURL))
                {
                    Skater newSkater = new Skater(playerURL);
                    allSkaters.Add(newSkater);
                    alreadyAdded.Add(playerURL);
                }
            }
            return allSkaters;
        }



        #endregion

    }
}
