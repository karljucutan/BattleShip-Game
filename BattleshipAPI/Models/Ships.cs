using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BattleshipAPI.Models
{
    public class Ships
    {
        public string Name { get; set; }
        public int Width { get; set; }

    }

    public class Destroyer : Ships
    {
        public Destroyer()
        {
            Name = "D";
            Width = 2;

        }
    }

    public class Submarine : Ships
    {
        public Submarine()
        {
            Name = "S";
            Width = 3;

        }
    }

    public class Cruiser : Ships
    {
        public Cruiser()
        {
            Name = "C";
            Width = 3;

        }
    }

    public class Battleship : Ships
    {
        public Battleship()
        {
            Name = "B";
            Width = 4;

        }
    }

    public class Carrier : Ships
    {
        public Carrier()
        {
            Name = "A";
            Width = 5;

        }
    }
}