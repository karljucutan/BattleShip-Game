using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleshipAPI.Models
{
    public class Player
    {
        public int ID { get; set; }
        public string[,] playerBoard { get; set; }
        public List<Ships> playerShips;
   
    }
}