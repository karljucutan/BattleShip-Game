using GameConsumer.Functionalities;
using GameConsumer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace GameConsumer
{
    public partial class Game : Form
    {
        Ships destroyer, submarine, cruiser, battleship, aircraftCarrier;

        Player player1, player2;

        public Game(Player _player1, Player _player2)
        {
            player1 = _player1;
            player2 = _player2;
            destroyer = new Destroyer();
            submarine = new Submarine();
            cruiser = new Cruiser();
            battleship = new Battleship();
            aircraftCarrier = new Carrier();
            InitializeComponent();            
        }

        void showPlayerBoard()
        {
            for(int x=0; x<player1.playerBoard.GetLength(0); x++)
            {
                for(int y=0; y<player1.playerBoard.GetLength(1); y++)
                {
                    ImageController.setImage(ImageController.getPicturebox(x, y, tableLayoutPanel_Player), player1.playerBoard[x,y]);
                }
            }
        }
            
        private void Game_Load(object sender, EventArgs e)
        {
            showPlayerBoard();
            foreach (PictureBox child in tableLayoutPanel_Enemy.Controls)
            {
                child.MouseClick += new MouseEventHandler(ClickedPBox);
            }
        }

        public void ClickedPBox(object sender, EventArgs e)
        {
            PictureBox selected = sender as PictureBox;
            int x = tableLayoutPanel_Enemy.GetRow(selected);
            int y = tableLayoutPanel_Enemy.GetColumn(selected);

            if (player2.playerBoard[x,y]!=null)
            {
                ImageController.setImage(ImageController.getPicturebox(x, y, tableLayoutPanel_Enemy), "Hit");
                switch(player2.playerBoard[x,y])
                {
                    case "D":
                        destroyer.Width--;
                        if (destroyer.Width <= 0)
                        {
                            showDestroyedShips("D", "#FFFF01");
                        }
                        break;
                    case "S":
                        submarine.Width--;
                        if (submarine.Width <= 0)
                        {
                            showDestroyedShips("S", "#FE0000");
                        }
                        break;
                    case "C":
                        cruiser.Width--;
                        if (cruiser.Width <= 0)
                        {
                            showDestroyedShips("C", "#00FF01");
                        }
                        break;
                    case "B":
                        battleship.Width--;
                        if (battleship.Width <= 0)
                        {
                            showDestroyedShips("B", "#0000FE");
                        }
                        break;
                    case "A":
                        aircraftCarrier.Width--;
                        if (aircraftCarrier.Width <= 0)
                        {
                            showDestroyedShips("A", "#BE00FE");
                        }
                        break;
                }
            }
            else
            {
                ImageController.setImage(ImageController.getPicturebox(x, y, tableLayoutPanel_Enemy), "Miss");
            }

            selected.Enabled = false;
        }

        void showDestroyedShips(string shipName, string colorStr)
        {
            for (int x = 0; x < player2.playerBoard.GetLength(0); x++)
            {
                for (int y = 0; y < player2.playerBoard.GetLength(1); y++)
                {
                    if (shipName == player2.playerBoard[x, y])
                        ImageController.getPicturebox(x, y, tableLayoutPanel_Enemy).BackColor = ColorTranslator.FromHtml(colorStr);
                }
            }
        }
    }
}
