using BattleshipAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace BattleshipAPI.Controllers
{
    public class MoveController : ApiController
    {
        

        [HttpGet]
        public List<Game> GetGames()
        {
            return Games.GAMES;
        }

        [HttpGet]
        public int GetJoinedGameID(int playerId)
        {
            foreach (Game game in Games.GAMES)
            {
                if (((game.player1.ID == playerId) || (game.player2.ID == playerId)) && game.gameState!=2)
                {
                    return game.gameId;
                }
            }
            return 0;
        }

        [HttpGet]
        public Game GetGameBoard(int playerId, int gameId)
        {
            var game = Games.GAMES.Where(u => (u.gameId.Equals(gameId))).SingleOrDefault();

            if ((game.player1.ID == playerId) || (game.player2.ID == playerId))
            {
                return game;
            }

            return null;
        }

        [HttpPost]
        public bool PostMove(int playerId, int gameId, int x, int y)
        {
            bool hit = false;
            var newPair = new KeyValuePair<int, int>(x, y);

            var game = Games.GAMES.Where(u => u.gameId.Equals(gameId)).SingleOrDefault();

            if (game.player1.ID == playerId)
            {
                game.lastPlayer1Move = newPair;
                if (game.player2Board[x, y] != null)
                {
                    hit = true;
                    ShipDestroyedChecker(game, game.player1.ID, x, y);

                }

                else
                {
                    hit = false; game.player2Board[x, y] = "MISS";
                }
                game.playerTurn = game.player2.ID.ToString();
                return hit;
            }

            else if (game.player2.ID == playerId)
            {
                game.lastPlayer2Move = newPair;
                if (game.player1Board[x, y] != null)
                {
                    hit = true;
                    ShipDestroyedChecker(game, game.player2.ID, x, y);

                }
                else
                {
                    hit = false; game.player1Board[x, y] = "MISS";
                }

                game.playerTurn = game.player1.ID.ToString();
                return hit;
            }

            return false;


    
        }
    

        private void ShipDestroyedChecker(Game game, int playerId, int x, int y)
        {
            Player enemy = new Player();
            if (game.player1.ID == playerId)
            {
                enemy.playerBoard = game.player2Board;
                enemy.playerShips = game.player2.playerShips;
            }
            else if (game.player2.ID == playerId)
            {
                enemy.playerBoard = game.player1Board;
                enemy.playerShips = game.player1.playerShips;
            }

          
            switch (enemy.playerBoard[x, y])
            {
                case "D":
                    enemy.playerShips.Where(s => s.Name == "D").FirstOrDefault().Width--;
                    break;
                case "S":
                    enemy.playerShips.Where(s => s.Name == "S").FirstOrDefault().Width--;
                    break;
                case "C":
                    enemy.playerShips.Where(s => s.Name == "C").FirstOrDefault().Width--;
                    break;
                case "B":
                    enemy.playerShips.Where(s => s.Name == "B").FirstOrDefault().Width--;
                    break;
                case "A":
                    enemy.playerShips.Where(s => s.Name == "A").FirstOrDefault().Width--;
                    break;
            }
            
            enemy.playerBoard[x, y] = "HIT";
            if (game.player1.ID == playerId)
            {
                game.player2Board = enemy.playerBoard;
                game.player2.playerShips = enemy.playerShips;
            }
            else if (game.player2.ID == playerId)
            {
                game.player1Board = enemy.playerBoard;
                game.player1.playerShips = enemy.playerShips;
            }

            if (
            enemy.playerShips.Where(s => s.Name == "D").Select(s => s.Width).FirstOrDefault() <= 0 &&
            enemy.playerShips.Where(s => s.Name == "S").Select(s => s.Width).FirstOrDefault() <= 0 &&
            enemy.playerShips.Where(s => s.Name == "C").Select(s => s.Width).FirstOrDefault() <= 0 &&
            enemy.playerShips.Where(s => s.Name == "B").Select(s => s.Width).FirstOrDefault() <= 0 &&
            enemy.playerShips.Where(s => s.Name == "A").Select(s => s.Width).FirstOrDefault() <= 0)
            {
                game.gameRunning = false;
                game.winner = playerId.ToString();
            }

        }


        [HttpPost]
        public bool JoinGame(int playerId)
        {
            try
            {
                Game foundGame = Games.GAMES.Where(u => u.gameState.Equals(0)).SingleOrDefault();
                if (foundGame == null)
                {
                    Game newGame = new Game();
                    newGame.gameId = ++Games.gameIdCounter;
                    newGame.player1 = new Player() { ID = playerId };
                    newGame.player1.playerShips = new List<Ships>();
                    newGame.player1.playerShips.Add(new Destroyer());
                    newGame.player1.playerShips.Add(new Submarine());
                    newGame.player1.playerShips.Add(new Cruiser());
                    newGame.player1.playerShips.Add(new Battleship());
                    newGame.player1.playerShips.Add(new Carrier());

                    Games.GAMES.Add(newGame);
                    return true;
                }
                else
                {
                    foundGame.player2 = new Player() { ID = playerId };
                    foundGame.player2.playerShips = new List<Ships>();
                    foundGame.player2.playerShips.Add(new Destroyer());
                    foundGame.player2.playerShips.Add(new Submarine());
                    foundGame.player2.playerShips.Add(new Cruiser());
                    foundGame.player2.playerShips.Add(new Battleship());
                    foundGame.player2.playerShips.Add(new Carrier());

                    foundGame.gameState = 1;
                    foundGame.gameRunning = true;
                    // random sino ang first attacker
                    int rand = RandomFirstTurn();
                    if (rand == 1)
                    {
                        foundGame.playerTurn = foundGame.player1.ID.ToString();
                    }
                    else if (rand == 2)
                    {
                        foundGame.playerTurn = foundGame.player2.ID.ToString();
                    }
                    return true;
                }
            }
            catch
            {
                return false;
            }

        }
        [HttpPost]
        public bool SetReady(int playerId, int gameId)
        {

            var game = Games.GAMES.Where(u => (u.gameId.Equals(gameId) && ((u.player1.ID == playerId) || (u.player2.ID == playerId)))).SingleOrDefault();
            if (playerId == game.player1.ID)
            { game.setup1 = true; return true; }
            else if (playerId == game.player2.ID)
            { game.setup2 = true; return true; }

            return false;
        }

     
        [HttpPost]
        public bool DeployShips(int playerId, int gameId, int x, int y, bool shipRotation, int currentShipWidth, string currentShipName) //int shipWidth)
        {

            string[,] boardSetup = new string[10, 10];
            var game = Games.GAMES.Where(u => (u.gameId.Equals(gameId) && ((u.player1.ID == playerId) || (u.player2.ID == playerId)))).SingleOrDefault();
            if (game.player1.ID == playerId)
            {
                boardSetup = game.player1Board;
            }
            else if (game.player2.ID == playerId)
            {
                boardSetup = game.player2Board;
            }

            if (shipRotation)
            {

                if (ArrayChecker(x, y, shipRotation, currentShipWidth, boardSetup))
                {
                    verticalDeploy(x, y, playerId, currentShipWidth, currentShipName, game);
                    return true;
                }

            }
            else
            {
                if (ArrayChecker(x, y, shipRotation, currentShipWidth, boardSetup))
                {
                    horizontalDeploy(x, y, playerId, currentShipWidth, currentShipName, game);
                    return true;

                }
            }
            return false;





        }
        
    
        private bool ArrayChecker(int x, int y, bool shipRotation, int shipWidth, string[,] boardSetup)
        { 
            bool success = false;
           
            if (boardSetup[x, y] == null)
            {
                for (int i = 0; i < shipWidth; i++) //iterate vertically based on shipWidth
                {
                    if (shipRotation)
                    {
                        if (boardSetup[x++, y] == null)
                        {
                            success = true;
                        }

                        else
                        { success = false; break; }
                    }
                    else
                    {
                        if (boardSetup[x, y++] == null)
                        {
                            success = true;
                        }

                        else
                        { success = false; break; }
                    }
                }//end of for loop

            }//end of ifBoardSetup
            
            return success;

        }

        private void verticalDeploy(int x, int y, int playerId, int currentShipWidth, string currentShipName, Game game)
        {
            for (int i = 0; i < currentShipWidth; i++)
            {
                if (game.player1.ID == playerId)
                {
                    game.player1Board[x,y] = currentShipName;
                }
                else if (game.player2.ID == playerId)
                {
                    game.player2Board[x,y] = currentShipName;
                }
               
         
                x++;//increments based on shipwidth
            }

        }

        private void horizontalDeploy(int x, int y, int playerId, int currentShipWidth, string currentShipName, Game game)
        {
            for (int i = 0; i < currentShipWidth; i++)
            {
                if (game.player1.ID == playerId)
                {
                    game.player1Board[x, y] = currentShipName;
                }
                else if (game.player2.ID == playerId)
                {
                    game.player2Board[x, y] = currentShipName;
                } //strings to represent board units
             
                y++;//increments based on shipwidth
            }
        }


        private int RandomFirstTurn()
        {
            int randomNumber = 0;
            Random randomizer = new Random();
            randomNumber = randomizer.Next(1,3);
            return randomNumber;
        }

    }
}
