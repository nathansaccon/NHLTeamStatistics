using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NHLTeamStats
{
    class Program
    {
        static void Main(string[] args)
        {

            NHLTeams league = new NHLTeams();

            foreach(NHLTeam team in league.AllTeams)
            {
                Console.WriteLine(team.AverageShotsFor());
            }
            Console.ReadKey();
        }
    }
}
