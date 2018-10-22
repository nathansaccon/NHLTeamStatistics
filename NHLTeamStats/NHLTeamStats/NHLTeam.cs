using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NHLTeamStats
{
    [Serializable]
    class NHLTeam
    {
        #region Class Variables

        string name;
        string abbreviation;
        List<Game> gamelog;

        public string Name { get => name; set => name = value; }
        public string Abbreviation { get => abbreviation; set => abbreviation = value; }
        public List<Game> Gamelog { get => gamelog; set => gamelog = value; }

        #endregion

        #region Constructor

        public NHLTeam()
        {

        }

        #endregion

        #region Averages

        #region Shot Averages
        /// <summary>
        /// Returns the average number of shots per game this season.
        /// </summary>
        /// <returns></returns>
        public float AverageShotsFor()
        {
            float totalShots = 0;
            foreach(Game game in Gamelog)
            {
                totalShots += game.shotsFor;
            }
            return (float)Math.Round((totalShots / Gamelog.Count), 2);
        }

        /// <summary>
        /// Returns the average number of shots per game for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float AverageShotsFor(float lastXGames)
        {
            float totalShots = 0;

            if (lastXGames >= Gamelog.Count)
            {
                return AverageShotsFor();
            } else
            {
                for (int i = 0; i < lastXGames; i++)
                {
                    totalShots += Gamelog[i].shotsFor;
                }
            }
            return (float)Math.Round((totalShots / lastXGames), 2); 
        }

        /// <summary>
        /// Returns the average number of shots against per game this season.
        /// </summary>
        /// <returns></returns>
        public float AverageShotsAgainst()
        {
            float totalShots = 0;
            foreach (Game game in Gamelog)
            {
                totalShots += game.shotsAgainst;
            }
            return (float)Math.Round((totalShots / Gamelog.Count), 2);
        }

        /// <summary>
        /// Returns the average number of shots against per game for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float AverageShotsAgainst(float lastXGames)
        {
            float totalShots = 0;

            if (lastXGames >= Gamelog.Count)
            {
                return AverageShotsAgainst();
            }
            else
            {
                for (int i = 0; i < lastXGames; i++)
                {
                    totalShots += Gamelog[i].shotsAgainst;
                }
            }
            return (float)Math.Round((totalShots / lastXGames), 2);
        }

        #endregion

        #region Goal Averages

        /// <summary>
        /// Returns the average number of goals per game this season.
        /// </summary>
        /// <returns></returns>
        public float AverageGoalsFor()
        {
            float totalGoals = 0;
            foreach (Game game in Gamelog)
            {
                totalGoals += game.goalsFor;
            }
            return (float)Math.Round((totalGoals / Gamelog.Count), 2);
        }

        /// <summary>
        /// Returns the average number of goals per game for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float AverageGoalsFor(float lastXGames)
        {
            float totalGoals = 0;

            if (lastXGames >= Gamelog.Count)
            {
                return AverageGoalsFor();
            }
            else
            {
                for (int i = 0; i < lastXGames; i++)
                {
                    totalGoals += Gamelog[i].goalsFor;
                }
            }
            return (float)Math.Round((totalGoals / lastXGames), 2);
        }

        /// <summary>
        /// Returns the average number of goals against per game this season.
        /// </summary>
        /// <returns></returns>
        public float AverageGoalsAgainst()
        {
            float totalGoals = 0;
            foreach (Game game in Gamelog)
            {
                totalGoals += game.goalsAgainst;
            }
            return (float)Math.Round((totalGoals / Gamelog.Count), 2);
        }

        /// <summary>
        /// Returns the average number of goals per game for the last X number of games.
        /// </summary>
        /// <param name="lastXGames"></param>
        /// <returns></returns>
        public float AverageGoalsAgainst(float lastXGames)
        {
            float totalGoals = 0;

            if (lastXGames >= Gamelog.Count)
            {
                return AverageGoalsAgainst();
            }
            else
            {
                for (int i = 0; i < lastXGames; i++)
                {
                    totalGoals += Gamelog[i].goalsAgainst;
                }
            }
            return (float)Math.Round((totalGoals / lastXGames), 2);
        }

        #endregion


        #endregion
    }
}
