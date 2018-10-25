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

            NHL NationalHockeyLeague = new NHL(true);

            foreach (Team[] teamPair in NationalHockeyLeague.Matchups())
            {
                Team home = teamPair[0];
                Team away = teamPair[1];

                Console.WriteLine(Team.TeamCategoryDifferenceCompare(home, away));
                Console.WriteLine(Team.TeamCategoryCompare(home, away)+"\n\n");
            }
            Console.ReadKey();
        }
    }
}
