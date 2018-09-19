using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameConsumer.Models
{
    public class Player
    {
        public int ID { get; set; }
        public string[,] playerBoard{ get; set; }
        public List<Ships> playerShips;
    
    }
}
