using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NHLTeamsLib;

/* Nathan Saccon NHLTeamStats Project
 *          Date Started: October 21, 2018: Created project from scratch
 * 
 */

namespace NHLTeamStats
{
    class Program
    {
        static void Main(string[] args)
        {

            NHLTeams league = new NHLTeams();

            foreach (NHLTeam[] teamPair in league.TodaysMatchups())
            {
                NHLTeam t1 = teamPair[0];
                NHLTeam t2 = teamPair[1];

                float t1goals;
                float t1goalsagainst;
                float t1shotsfor;
                float t1shotsagainst;
                float t2goals;
                float t2goalsagainst;
                float t2shotsfor;
                float t2shotsagainst;
                if (t1.NextGameArena == Arena.HOME)
                {
                    t1goals = t1.HomeGoalsForPerGame;
                    t1goalsagainst = t1.HomeGoalsAgainstPerGame;
                    t1shotsfor = t1.HomeShotsForPerGame;
                    t1shotsagainst = t1.HomeShotsAgainstPerGame;

                    t2goals = t2.AwayGoalsForPerGame;
                    t2goalsagainst = t2.AwayGoalsAgainstPerGame;
                    t2shotsfor = t2.AwayShotsForPerGame;
                    t2shotsagainst = t2.AwayShotsAgainstPerGame;
                }
                else
                {
                    t1goals = t1.AwayGoalsForPerGame;
                    t1goalsagainst = t1.AwayGoalsAgainstPerGame;
                    t1shotsfor = t1.AwayShotsForPerGame;
                    t1shotsagainst = t1.AwayShotsAgainstPerGame;

                    t2goals = t2.HomeGoalsForPerGame;
                    t2goalsagainst = t2.HomeGoalsAgainstPerGame;
                    t2shotsfor = t2.HomeShotsForPerGame;
                    t2shotsagainst = t2.HomeShotsAgainstPerGame;
                }
                Console.WriteLine(t1.Name + "    vs    " + t2.Name);
                Console.WriteLine(t1goals + "       " + t2.GoalsForPerGame);
                Console.WriteLine(t1goalsagainst + "       " + t2.GoalsAgainstPerGame);
                Console.WriteLine(t1shotsfor + "       " + t2.ShotsForPerGame);
                Console.WriteLine(t1shotsagainst + "       " + t2.ShotsAgainstPerGame);
                Console.WriteLine(t1.NextGameArena + "       " + t2.NextGameArena+"\n\n\n");
            }
            Console.ReadKey();
        }
    }
}
