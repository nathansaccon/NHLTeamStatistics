using NHLTeamsLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TeamsNHL
{
    [Serializable]
    public class Skater : Player
    {
        #region Class Variables

        // DraftKings
        internal string draftKingsName;
        internal string draftKingsPosition;
        internal int draftKingsCost;
        // FanDuel
        internal string fanDuelName;
        internal string fanDuelPosition;
        internal int fanDuelCost;

        internal float startTime;
        internal bool isInjured = false;

        #endregion

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
            PopulateFantasyData();
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

        #region Populate Fantasy Stats

        /// <summary>
        /// Populates Skater with their fantasy data
        /// </summary>
        private void PopulateFantasyData()
        {
            DraftKingsDataPopulate();
            FanDuelDataPopulate();
        }

        /// <summary>
        /// Populates player with their DraftKings fantasy data
        /// </summary>
        private void DraftKingsDataPopulate()
        {
            StreamReader reader = new StreamReader(NHL.DRAFTKINGS_FILE);
            reader.ReadLine();
            bool nameFound = false;
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(',');
                string dkPosition = line[0];
                string dkName = line[2];
                string dkCost = line[5];
                float startTime = LineToTimeDecimal(line[6]);
                
                if (NameCorrection(dkName) == Name)
                {
                    draftKingsName = dkName;
                    draftKingsPosition = dkPosition;
                    draftKingsCost = Convert.ToInt32(dkCost);
                    this.startTime = startTime;
                    nameFound = true;
                    break;
                }
            }
            if (NHL.TEST && !nameFound && gamelog.Count > 1)
            {
                Console.WriteLine(Name + ": " + TeamAbbreviation);
            }
        }

        /// <summary>
        /// Populates player with their DraftKings fantasy data
        /// </summary>
        private void FanDuelDataPopulate()
        {
            StreamReader reader = new StreamReader(NHL.FANDUEL_FILE);
            reader.ReadLine();
            bool nameFound = false;
            while (!reader.EndOfStream)
            {
                string[] line = reader.ReadLine().Split(',');
                string fdName = RemoveStringSymbols(line[3]);

                if (NameCorrection(fdName) == Name)
                {
                    string fdCost = RemoveStringSymbols(line[7]);
                    string fdPosition = RemoveStringSymbols(line[1]);

                    fanDuelName = fdName;
                    fanDuelPosition = fdPosition;
                    fanDuelCost = Convert.ToInt32(fdCost);
                    isInjured = line[11] == "\"\"";
                    nameFound = true;
                    break;
                }
            }
            if (NHL.TEST && !nameFound && gamelog.Count > 1)
            {
                Console.WriteLine(Name + ": " + TeamAbbreviation);
            }
        }

        /// <summary>
        /// Removes string ""'s from a string
        /// </summary>
        /// <param name="sample"></param>
        /// <returns></returns>
        private string RemoveStringSymbols(string sample)
        {
            return sample.Split('"')[1];
        }

        /// <summary>
        /// Returns the time as a decimal so 7:30 is 7.5
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private float LineToTimeDecimal(string line)
        {
            float answer = 0;
            string time = line.Split(' ')[2];
            answer += Convert.ToInt32(time.Split(':')[0]);

            string minute = time.Split(':')[1];
            if (minute[0].ToString() == "3")
            {
                answer += 0.5f;
            }

            return answer;

        }

        /// <summary>
        /// Returns the hockey reference name of a player
        /// </summary>
        /// <param name="dkName"></param>
        /// <returns></returns>
        private string NameCorrection(string dkName)
        {
            string correctedName = dkName;
            if (dkName == "Jonathan Marchessault") { correctedName = "Jon Marchessault"; }
            else if (dkName == "Matt Dumba") { correctedName = "Mathew Dumba"; }
            else if (dkName == "Jake DeBrusk") { correctedName = "Jake Debrusk"; }
            else if (dkName == "Tony DeAngelo") { correctedName = "Anthony DeAngelo"; }
            else if (dkName == "Jacob De La Rose") { correctedName = "Jacob de La Rose"; }
            else if (dkName == "Jon Merrill") { correctedName = "Jonathon Merrill"; }
            return correctedName;
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

        #region Fantasy Hockey Methods

        /// <summary>
        /// Returns the draftkings player as HTML
        /// </summary>
        /// <returns></returns>
        internal string ToDraftKingsHTML()
        {
            string skaterAsHTML = "";

            skaterAsHTML += $"<h4>{Name}</h4>\n";
            skaterAsHTML += $"<h2>{TeamAbbreviation}</h2>";
            skaterAsHTML += $"<p>DraftKings Position: {draftKingsPosition}<br />\nDK Cost: {draftKingsCost}</p>";
            skaterAsHTML += StatToHTML("Points Per Game", Assists + Goals);
            skaterAsHTML += StatToHTML("Shots Per Game", Shots);
            skaterAsHTML += StatToHTML("TOI Per Game", TimeOnIce);
            skaterAsHTML += StatToHTML("Hits Per Game", Hits);
            skaterAsHTML += StatToHTML("Blocks Per Game", Blocks);
            skaterAsHTML += StatToHTML("Value", DraftKingsValue());

            return skaterAsHTML;
        }

        /// <summary>
        /// Returns the draftkings player as HTML
        /// </summary>
        /// <returns></returns>
        internal string ToFanDuelHTML()
        {
            string skaterAsHTML = "";

            skaterAsHTML += $"<h4>{Name}</h4>\n";
            skaterAsHTML += $"<h2>{TeamAbbreviation}</h2>";
            skaterAsHTML += $"<p>FanDuel Position: {fanDuelPosition}<br />\nFD Cost: {fanDuelCost}</p>";
            skaterAsHTML += StatToHTML("Points Per Game", Assists + Goals);
            skaterAsHTML += StatToHTML("Shots Per Game", Shots);
            skaterAsHTML += StatToHTML("TOI Per Game", TimeOnIce);
            skaterAsHTML += StatToHTML("Hits Per Game", Hits);
            skaterAsHTML += StatToHTML("Blocks Per Game", Blocks);
            skaterAsHTML += StatToHTML("Value", FanDuelValue());

            return skaterAsHTML;
        }

        /// <summary>
        /// Returns a stat as an HTML formatted string.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="stat"></param>
        /// <returns></returns>
        private static string StatToHTML(string title, float stat)
        {
            return $"<p>{title}: <b style=\"font-size:1.2em;\">{Math.Round(stat, 2).ToString()}</b></p>\n";
        }

        /// <summary>
        /// Returns the value of this player
        /// </summary>
        /// <returns></returns>
        internal float DraftKingsValue()
        {
            float value = 0;
            if (draftKingsCost != 0)
            {
                value =  10000 * DraftKingsAverage() / draftKingsCost;
            }
            return value;
        }

        /// <summary>
        /// Returns the value of this player
        /// </summary>
        /// <returns></returns>
        internal float DraftKingsValue(Arena arena)
        {
            float value = 0;
            if (draftKingsCost != 0)
            {
                value = 10000 * DraftKingsAverage(arena) / draftKingsCost;
            }
            return value;
            
        }

        /// <summary>
        /// Returns the value of this player
        /// </summary>
        /// <returns></returns>
        internal float FanDuelValue()
        {
            float value = 0;
            if (fanDuelCost != 0)
            {
                value = 10000 * FanDuelAverage() / fanDuelCost;
            }
            return value;
        }

        /// <summary>
        /// Returns the value of this player
        /// </summary>
        /// <returns></returns>
        internal float FanDuelValue(Arena arena)
        {
            float value = 0;
            if (fanDuelCost != 0)
            {
                value = 10000 * FanDuelAverage(arena) / fanDuelCost;
            }
            return value;
        }

        /// <summary>
        /// Returns the average points this player earns on DraftKings
        /// </summary>
        /// <returns></returns>
        private float DraftKingsAverage()
        {
            const float GOAL = 3;
            const float ASSIST = 2;
            const float SHOT = 0.5f;
            const float BLOCK = 0.5f;

            return Goals * GOAL + Assists * ASSIST + Shots * SHOT + Blocks + BLOCK;
        }

        /// <summary>
        /// Returns the average points this player earns on DraftKings at home/away
        /// </summary>
        /// <returns></returns>
        private float DraftKingsAverage(Arena arena)
        {
            const float GOAL = 3;
            const float ASSIST = 2;
            const float SHOT = 0.5f;
            const float BLOCK = 0.5f;

            if (arena == Arena.HOME)
            {
                return HomeGoals * GOAL + HomeAssists * ASSIST + HomeShots * SHOT + HomeBlocks * BLOCK;
            } else
            {
                return AwayGoals * GOAL + AwayAssists * ASSIST + AwayShots * SHOT + AwayBlocks * BLOCK;
            }
        }

        /// <summary>
        /// Returns the average points this player earns on DraftKings
        /// </summary>
        /// <returns></returns>
        private float FanDuelAverage()
        {
            const float GOAL = 12;
            const float ASSIST = 8;
            const float SHOT = 1.6f;
            const float BLOCK = 1.6f;

            return Goals * GOAL + Assists * ASSIST + Shots * SHOT + Blocks + BLOCK;
        }

        /// <summary>
        /// Returns the average points this player earns on DraftKings at home/away
        /// </summary>
        /// <returns></returns>
        private float FanDuelAverage(Arena arena)
        {
            const float GOAL = 12;
            const float ASSIST = 8;
            const float SHOT = 1.6f;
            const float BLOCK = 1.6f;

            if (arena == Arena.HOME)
            {
                return HomeGoals * GOAL + HomeAssists * ASSIST + HomeShots * SHOT + HomeBlocks * BLOCK;
            }
            else
            {
                return AwayGoals * GOAL + AwayAssists * ASSIST + AwayShots * SHOT + AwayBlocks * BLOCK;
            }
        }

        #endregion
    }
}
