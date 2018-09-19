using GameConsumer.Models;
using GameConsumer.Services;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;

namespace GameConsumer
{
    public partial class Login : Form
    {
        ServiceClientWrapper client;

        public Login()
        {
            InitializeComponent();
            client = new ServiceClientWrapper();
           
        }
      
        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && (e.KeyChar == '.'))
            {
                e.Handled = true;
            }
        }
     

        private void button_JoinGame_Click(object sender, EventArgs e)
        {
            Player player = new Player();
            int playerid;
            if (int.TryParse(textBox1.Text, out playerid))
            {
                player.ID = playerid;
                //var baseAddress = "http://localhost:9090/api/Move/JoinGame";
#if DEBUG
                var baseAddress = Config.BASE_ADDRESS_DEBUG + "Move/JoinGame";
#else
                var baseAddress = Config.BASE_ADDRESS + "Move/JoinGame";
#endif
                var parameters = new Dictionary<string, string>
                {
                    {"playerId", player.ID.ToString()},
                };

                var result = client.Send(new ServiceRequest { BaseAddress = baseAddress, HttpProtocol = Protocols.HTTP_POST, RequestParameters = parameters });
                var success = JsonConvert.DeserializeObject<Boolean>(result.Response);

                if (success)
                {
                    Setup setup = new Setup(player.ID);
                    this.Hide();
                    setup.Show();
                    MessageBox.Show("Game ID: "+ sendRequest());
                }
                
            }
            
            else
            { MessageBox.Show("Hey, we need an int over here."); }
        }

        private string sendRequest()
        {

            //var baseAddress = "http://localhost:9090/api/Move/GetGames";

#if DEBUG
            var baseAddress = Config.BASE_ADDRESS_DEBUG + "Move/GetGames";
#else
            var baseAddress = Config.BASE_ADDRESS + "Move/GetGames";
#endif

            var result = client.Send(new ServiceRequest { BaseAddress = baseAddress, HttpProtocol = Protocols.HTTP_GET});
            var games = JsonConvert.DeserializeObject<List<Game>>(result.Response);

            return games.Last().gameId.ToString();
        }
    }
}
