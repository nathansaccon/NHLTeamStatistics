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

            NHL NationalHockeyLeague = new NHL(loadTeams:false, loadSkaters:true);
            int DAYSAHEAD = 0;
            NationalHockeyLeague.WriteMatchupHTML(DAYSAHEAD);
            NationalHockeyLeague.WriteDraftKingsValueHTML(DAYSAHEAD);
            NationalHockeyLeague.WriteFanDuelValueHTML(DAYSAHEAD);

        }
    }
}
