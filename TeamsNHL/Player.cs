using NHLTeamsLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamsNHL
{
    [Serializable]
    public abstract class Player
    {
        protected string name;
        protected string url;
        protected string teamAbbreviation;
        protected string position;
        protected List<Game> gamelog;

        public string Name { get => name; }
        public string Url { get => url; set => url = value; }
        public string TeamName { get => teamAbbreviation; }
        public string Position { get => position; }
    }
}
