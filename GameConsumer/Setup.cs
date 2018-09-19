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
using System.Media;



namespace GameConsumer
{

    public partial class Setup : Form
    {
        ServiceClientWrapper client;
        Ships currentShip;
        public bool shipRotation;
        Player player = new Player();
        SoundPlayer soundPlayer;

        public int gameId;

        public Setup(int playerId)
        {
            InitializeComponent();
            player.ID = playerId;
            client = new ServiceClientWrapper();
            gameId = GetGameId();
        }

        public void debugValues()
        {
            shipRotation = true;
            currentShip = new Destroyer();
            DeployShips(player.ID, gameId, 0, 0, shipRotation, currentShip);
            verticalDeploy(0, 0);
            button_shipDestroyer.Enabled = false;
            currentShip = new Submarine();
            //&& !button_shipCruiser.Enabled && !button_shipCarrier.Enabled && !button_shipBattleship.Enabled
            DeployShips(player.ID, gameId, 0, 1, shipRotation, currentShip);
            verticalDeploy(0, 1);
            button_shipSubmarine.Enabled = false;
            currentShip = new Cruiser();
            DeployShips(player.ID, gameId, 0, 2, shipRotation, currentShip);
            verticalDeploy(0, 2);
            button_shipCruiser.Enabled = false;
            currentShip = new Battleship();
            shipRotation = false;
            DeployShips(player.ID, gameId, 0, 3, shipRotation, currentShip);
            horizontalDeploy(0, 3);
            button_shipBattleship.Enabled = false;
            shipRotation = true;
            currentShip = null;

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            soundPlayer = new SoundPlayer(GameConsumer.Properties.Resources.cave_disco);
            soundPlayer.Play();
          
            shipRotation = true;
            label_gameID.Text += gameId.ToString();
            label_playerID.Text += player.ID.ToString();
            foreach (PictureBox child in tableLayoutPanel1.Controls)
            {
                child.MouseClick += new MouseEventHandler(ClickedPBox);
            }

           // debugValues(); //setup board for debug
        }

        private void ClickedPBox(object sender, EventArgs e)
        {
            try {
                PictureBox selected = sender as PictureBox;
                int x = tableLayoutPanel1.GetRow(selected);
                int y = tableLayoutPanel1.GetColumn(selected);

                if (shipRotation)
                {
                    if (DeployShips(player.ID, gameId, x, y, shipRotation, currentShip))
                    {
                        verticalDeploy(x, y);
                        disableShipButton(currentShip.Name);
                    }
                }
                else
                {
                    if (DeployShips(player.ID, gameId, x, y, shipRotation, currentShip))
                    {
                        horizontalDeploy(x, y);
                        disableShipButton(currentShip.Name);
                    }
                }
            }
            catch (Exception)
            { }

        }



        public bool DeployShips(int playerId, int gameId, int x, int y, bool shipRotation, Ships currentShip)
        {
            string rotation;
            if (shipRotation == true)
            { rotation = "true"; }
            else
            { rotation = "false"; }

            //var setupAddress = "http://localhost:9090/api/Move/DeployShips";
#if DEBUG
            var setupAddress = Config.BASE_ADDRESS_DEBUG + "Move/DeployShips";
#else
            var setupAddress = Config.BASE_ADDRESS + "Move/DeployShips";
#endif

            var setupParams = new Dictionary<string, string>
                    {
                        {"playerId", playerId.ToString()},
                        {"gameId", gameId.ToString()},
                        {"x", x.ToString()},
                        {"y", y.ToString()},
                        {"shipRotation", rotation},
                        {"currentShipWidth", currentShip.Width.ToString()},
                        {"currentShipName", currentShip.Name}
                    };

            //var body = boardSetup.ToString();
            var result = client.Send(new ServiceRequest { BaseAddress = setupAddress, HttpProtocol = Protocols.HTTP_POST, RequestParameters = setupParams/* , Body = body*/});
            var success = JsonConvert.DeserializeObject<Boolean>(result.Response);

            return success;
        }



        void verticalDeploy(int x, int y)
        {
            for (int i = 0; i < currentShip.Width; i++)
            {
                ImageController.setImage(ImageController.getPicturebox(x, y, tableLayoutPanel1), currentShip.Name);
                x++;//increments based on shipwidth
            }

        }

        void horizontalDeploy(int x, int y)
        {
            for (int i = 0; i < currentShip.Width; i++)
            {

                ImageController.setImage(ImageController.getPicturebox(x, y, tableLayoutPanel1), currentShip.Name);
                y++;//increments based on shipwidth
            }
        }

        private void disableShipButton(String shipName)
        {
            switch (shipName)
            {
                case "D": button_shipDestroyer.Enabled = false; break;
                case "S": button_shipSubmarine.Enabled = false; break;
                case "C": button_shipCruiser.Enabled = false; break;
                case "B": button_shipBattleship.Enabled = false; break;
                case "A": button_shipCarrier.Enabled = false; break;
                default: break;
            }
            currentShip = null;
        }

        private void button_shipDestroyer_Click(object sender, EventArgs e)
        {
            currentShip = new Destroyer();
        }

        private void button_shipSubmarine_Click(object sender, EventArgs e)
        {
            currentShip = new Submarine();
        }

        private void button_shipCruiser_Click(object sender, EventArgs e)
        {
            currentShip = new Cruiser();
        }

        private void button_shipBattleship_Click(object sender, EventArgs e)
        {
            currentShip = new Battleship();
        }

        private void button_shipCarrier_Click(object sender, EventArgs e)
        {
            currentShip = new Carrier();
        }

        private void button_Rotate_Click(object sender, EventArgs e)
        {
            shipRotation = !shipRotation;
            if (shipRotation)
                orientationLabel.Text = "Vertical";
            else
                orientationLabel.Text = "Horizontal";
        }

        public void button_ready_Click(object sender, EventArgs e)
        {
            if (!button_shipDestroyer.Enabled && !button_shipSubmarine.Enabled && !button_shipCruiser.Enabled && !button_shipCarrier.Enabled && !button_shipBattleship.Enabled)
            {
                //player.playerBoard = boardSetup;
                // may mag rereturn ng boolean if nag ready na both players tapos call na yung game form
                //Game game = new Game(player);// pass to webservice with boolean ready ?WHAT
                //game.Show();
                if (SetReady(player.ID, gameId))
                {
                    button_ready.Text = "Waiting";
                    while (true)
                    {
                        Game game = new Game();
                        game = GetGame();

                        if (game.setup1 == true && game.setup2 == true)
                        {
                            soundPlayer.Stop();
                            soundPlayer.Dispose();
                            GameForm gameform = new GameForm(game, player.ID);
                            gameform.Show();
                            this.Hide();

                            break;
                            
                        }
                    }
                }
   
                
            }
            else
            { MessageBox.Show("Deploy all ships!"); }
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

           
            var result = client.Send(new ServiceRequest { BaseAddress = setupAddress, HttpProtocol = Protocols.HTTP_GET, RequestParameters = setupParams});
            var game = JsonConvert.DeserializeObject<Game>(result.Response);
            
            return game;
        }

       
        private int GetGameId()
        {
            //var getgameAddress = "http://localhost:9090/api/Move/GetJoinedGameID";
#if DEBUG
            var getgameAddress = Config.BASE_ADDRESS_DEBUG + "Move/GetJoinedGameID";
#else
            var getgameAddress = Config.BASE_ADDRESS + "Move/GetJoinedGameID";
#endif
            var getgameParams = new Dictionary<string, string>
                {
                    {"playerId", player.ID.ToString()}
                };

            var result = client.Send(new ServiceRequest { BaseAddress = getgameAddress, HttpProtocol = Protocols.HTTP_GET, RequestParameters = getgameParams });
            var gameId = JsonConvert.DeserializeObject<int>(result.Response);
            return gameId;
        }

        private bool SetReady(int playerId, int _gameId)
        {
            //var getgameAddress = "http://localhost:9090/api/Move/SetReady";
#if DEBUG
            var getgameAddress = Config.BASE_ADDRESS_DEBUG + "Move/SetReady";
#else
            var getgameAddress = Config.BASE_ADDRESS + "Move/SetReady";
#endif
            var getgameParams = new Dictionary<string, string>
                {
                    {"playerId", player.ID.ToString()},
                    {"gameId", _gameId.ToString()}
                };

            var result = client.Send(new ServiceRequest { BaseAddress = getgameAddress, HttpProtocol = Protocols.HTTP_POST, RequestParameters = getgameParams });
            var success = JsonConvert.DeserializeObject<bool>(result.Response);
            
            return success;
        }
    }
}
