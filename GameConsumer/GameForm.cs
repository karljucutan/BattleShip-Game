using GameConsumer.Functionalities;
using GameConsumer.Models;
using GameConsumer.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Media;


namespace GameConsumer
{
    public partial class GameForm : Form
    {

        int gameId;
        Player player, enemy;
        ServiceClientWrapper client;
        Game runningGame;
        SoundPlayer soundPlayer;


        public GameForm(Game game, int playerId)
        {
          
            runningGame = new Game();
            client = new ServiceClientWrapper();
            gameId = game.gameId;
            player = new Player();
            enemy = new Player();
            player.ID = playerId;

            if (game.player1.ID == playerId)
            {
                player.playerBoard = game.player1Board;
                enemy.playerBoard = game.player2Board;
                enemy.ID = game.player2.ID;
               
            }
            else if (game.player2.ID == playerId)
            {
                player.playerBoard = game.player2Board;
                enemy.playerBoard = game.player1Board;
                enemy.ID = game.player1.ID;
              
            }
            runningGame = game;

            InitializeComponent();
      
        }
 
        private void ShowPlayerBoard()
        {
            for(int x=0; x<player.playerBoard.GetLength(0); x++)
            {
                for(int y=0; y<player.playerBoard.GetLength(1); y++)
                {
                    ImageController.setBGColor(ImageController.getPicturebox(x, y, tableLayoutPanel_Player), player.playerBoard[x,y]);
                }
            }
        }
        private void UpdatePlayerBoard(int row, int col)
        {
            if (player.playerBoard[row, col] == "HIT")
            {
                ImageController.setImage(ImageController.getPicturebox(row, col, tableLayoutPanel_Player), "Hit");
            }
            else if (player.playerBoard[row, col] == "MISS")
            { ImageController.setImage(ImageController.getPicturebox(row, col, tableLayoutPanel_Player), "Miss"); }
         }

        private void UpdateEnemyBoard(int key, int value)
        {
            switch (enemy.playerBoard[key, value])
            {
                case "D":
                    var width = enemy.playerShips.Where(s => s.Name == "D").Select(s => s.Width).FirstOrDefault();
                    if (width <= 0)
                    {
                        showDestroyedShips("D", "#FFFF01");

                    }
                    break;
                case "S":
                    if (enemy.playerShips.Where(s => s.Name == "S").Select(s => s.Width).FirstOrDefault() <= 0)
                    {
                        showDestroyedShips("S", "#FE0000");
                    }
                    break;
                case "C":
                    if (enemy.playerShips.Where(s => s.Name == "C").Select(s => s.Width).FirstOrDefault() <= 0)
                    {
                        showDestroyedShips("C", "#00FF01");
                    }
                    break;
                case "B":
                    if (enemy.playerShips.Where(s => s.Name == "B").Select(s => s.Width).FirstOrDefault() <= 0)
                    {
                        showDestroyedShips("B", "#0000FE");
                    }
                    break;
                case "A":
                    if (enemy.playerShips.Where(s => s.Name == "A").Select(s => s.Width).FirstOrDefault() <= 0)
                    {
                        showDestroyedShips("A", "#BE00FE");
                    }
                    break;

            }


        }

        private Game GetGameAlways(Game game)
        {
            runningGame = game;
            
            if (runningGame.player1.ID == player.ID)
            {
                player.playerBoard = runningGame.player1Board;
                enemy.playerShips = runningGame.player2.playerShips;
               
            }
            else if (runningGame.player2.ID == player.ID)
            {
                player.playerBoard = runningGame.player2Board;
                enemy.playerShips = runningGame.player1.playerShips;
             
            }
            return runningGame;
        }
        private void Show(Game _runningGame)
        {
            if (_runningGame.gameRunning)
            {
                label_WhosTurn.Text = _runningGame.playerTurn + "'s Turn";
            }
            else
            {
                label_WhosTurn.Text = "Player " + _runningGame.winner + " Wins!";
                label_WhosTurn.ForeColor = Color.Red;
                button_playAgain.Enabled = true;
   
            }


            if (_runningGame.player1.ID == player.ID)
            {
                //playerboard
                UpdatePlayerBoard(_runningGame.lastPlayer2Move.Key, _runningGame.lastPlayer2Move.Value);
                //enemyboard
                UpdateEnemyBoard(_runningGame.lastPlayer1Move.Key, _runningGame.lastPlayer1Move.Value);
                

            }
            else if (_runningGame.player2.ID == player.ID)
            {

                UpdatePlayerBoard(_runningGame.lastPlayer1Move.Key, _runningGame.lastPlayer1Move.Value);
                UpdateEnemyBoard(_runningGame.lastPlayer2Move.Key, _runningGame.lastPlayer2Move.Value);

            }

        }

        private void Game_Load(object sender, EventArgs e)
        {
            button_playAgain.Enabled = false;
            label_SinongPlayerInYourBoard.Text = "Player ID: " + player.ID;
            label_SinongPlayerInEnemyBoard.Text = "Enemy ID: " + enemy.ID;

            ShowPlayerBoard();
           
            foreach (PictureBox child in tableLayoutPanel_Enemy.Controls)
            {
                child.MouseClick += new MouseEventHandler(ClickedPBox);
            }  
        }

        async Task PutTaskDelay(int time)
        {
            await Task.Delay(time);
        }

        private void GameForm_Shown(object sender, EventArgs e)
        {
            var token = new CancellationTokenSource();
            var listener = Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    if (runningGame.gameRunning == false)
                    {
                        token.Cancel();
                    }
                    Action<Game> windowDelegate = Show;
                    Invoke(windowDelegate, GetGameAlways(GetGame()));


                    await PutTaskDelay(100);

                    if (token.IsCancellationRequested)
                    {
                        break;
                    }
                }

            }, token.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

      
        
       
        public void ClickedPBox(object sender, EventArgs e)
        {
            if (runningGame.player1.ID == player.ID && runningGame.playerTurn.Equals(player.ID.ToString()) && runningGame.gameRunning)
            {
                PictureBoxIsClickedAsync(sender, e);                         
            }
            else if (runningGame.player2.ID == player.ID && runningGame.playerTurn.Equals(player.ID.ToString()) && runningGame.gameRunning)
            {
                PictureBoxIsClickedAsync(sender, e);
            }
        }

    

        private async void PictureBoxIsClickedAsync(object sender, EventArgs e)
        {
            PictureBox selected = sender as PictureBox;
            int x = tableLayoutPanel_Enemy.GetRow(selected);
            int y = tableLayoutPanel_Enemy.GetColumn(selected);
            soundPlayer = new SoundPlayer(GameConsumer.Properties.Resources.shotSound);
            soundPlayer.Play();
            selected.Parent.Enabled = false;//disable tableLayoutPanel para di maka double attack
            await PutTaskDelay(2200);

            if (IsHit(x,y))
            {
                ImageController.setImage(ImageController.getPicturebox(x, y, tableLayoutPanel_Enemy), "Hit");
                soundPlayer = new SoundPlayer(GameConsumer.Properties.Resources.hitSound);
                soundPlayer.Play();

            }
            else
            {
                soundPlayer = new SoundPlayer(GameConsumer.Properties.Resources.splashSound);
                soundPlayer.Play();
                ImageController.setImage(ImageController.getPicturebox(x, y, tableLayoutPanel_Enemy), "Miss");
            }

            selected.Parent.Enabled = true;//reenable tableLayoutPanel para maka attack next turn
            selected.Enabled = false; //disable para di na maka attack ulit ng same coords
        }

        private void button_playAgain_Click(object sender, EventArgs e)
        {
            Login newlogin = new Login();
            newlogin.Show();
            this.Hide();
        }

        void showDestroyedShips(string shipName, string colorStr)
        {
            for (int x = 0; x < enemy.playerBoard.GetLength(0); x++)
            {
                for (int y = 0; y < enemy.playerBoard.GetLength(1); y++)
                {
                    if (shipName == enemy.playerBoard[x, y])
                        ImageController.getPicturebox(x, y, tableLayoutPanel_Enemy).BackColor = ColorTranslator.FromHtml(colorStr);
                }
            }
        }

        
        private bool IsHit(int x, int y)
        {
#if DEBUG
            var setupAddress = Config.BASE_ADDRESS_DEBUG + "Move/PostMove";
#else
            var setupAddress = Config.BASE_ADDRESS + "Move/PostMove";
#endif
            var setupParams = new Dictionary<string, string>
                {
                    {"playerId", player.ID.ToString()},
                    {"gameId",  gameId.ToString()},
                    {"x", x.ToString()},
                    {"y", y.ToString()}
                };
        
            var result = client.Send(new ServiceRequest { BaseAddress = setupAddress, HttpProtocol = Protocols.HTTP_POST, RequestParameters = setupParams });
            var success = JsonConvert.DeserializeObject<bool>(result.Response);

            return success;
        }


        private Game GetGame()
        {

            //var setupAddress = "http://localhost:9090/api/Move/GetGameBoard";
#if DEBUG
            var setupAddress = Config.BASE_ADDRESS_DEBUG + "Move/GetGameBoard";
#else
            var setupAddress = Config.BASE_ADDRESS + "Move/GetGameBoard";
#endif
            var setupParams = new Dictionary<string, string>
                {
                    {"playerId", player.ID.ToString()},
                    {"gameId",  gameId.ToString()}
                };


            var result = client.Send(new ServiceRequest { BaseAddress = setupAddress, HttpProtocol = Protocols.HTTP_GET, RequestParameters = setupParams });
            var game = JsonConvert.DeserializeObject<Game>(result.Response);

            return game;
        }

        
    }
}
