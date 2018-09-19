using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleshipAPI.Models
{
    public class GameBase
    {
        public int gameId;
        public Player player1, player2;
        public bool setup1 = false, setup2 = false;
        public string playerTurn;
        public KeyValuePair<int, int> lastPlayer1Move;
        public KeyValuePair<int, int> lastPlayer2Move;
        public int gameState = 0;
    }
}