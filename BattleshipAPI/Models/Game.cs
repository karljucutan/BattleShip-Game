using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleshipAPI.Models
{
    public class Game : GameBase
    {
        public string[,] player1Board = new string[10,10];
        public string[,] player2Board = new string[10,10];
        public bool gameRunning = false;
        public string winner = "";
    }
}