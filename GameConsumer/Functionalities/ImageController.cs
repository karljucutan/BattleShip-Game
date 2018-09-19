using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameConsumer.Functionalities
{
    class ImageController
    {
        public static PictureBox getPicturebox(int x, int y, TableLayoutPanel tbl)
        {
            return (PictureBox)tbl.GetControlFromPosition(y, x);//GetControlFromPosition(column, row), pag nag null to ibig sabihin out of bounds
        }

        public static void setImage(PictureBox pb, String shipName)
        {
            switch (shipName)
            {
                case "D": pb.Image = Properties.Resources.D; break; //
                case "S": pb.Image = Properties.Resources.S; break; //
                case "C": pb.Image = Properties.Resources.C; break; // used forshowEnemyBoard only
                case "B": pb.Image = Properties.Resources.B; break; //
                case "A": pb.Image = Properties.Resources.A; break; //
                case "Hit": pb.Image = Properties.Resources.hitImage; break;
                case "Miss": pb.Image = Properties.Resources.splashImage; break;
                default: break;
            }
        }

        public static void setBGColor(PictureBox pb, string shipName)
        {
            switch (shipName)
            {
                case "D": pb.BackColor = ColorTranslator.FromHtml("#FFFF01"); break;
                case "S": pb.BackColor = ColorTranslator.FromHtml("#FE0000"); break;
                case "C": pb.BackColor = ColorTranslator.FromHtml("#00FF01"); break;
                case "B": pb.BackColor = ColorTranslator.FromHtml("#0000FE"); break;
                case "A": pb.BackColor = ColorTranslator.FromHtml("#BE00FE"); break;
                default: break;
            }
        }
    }
}
