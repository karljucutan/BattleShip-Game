using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleshipAPI.Models
{
    public class Games
    {
        public static List<Game> GAMES = new List<Game>();
        public static int gameIdCounter = 0;
    }
}