using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plotter
{
    public partial class FormColMap : Form
    {
        public FormColMap()
        {
            InitializeComponent();
        }

        private void FormColMap_Load(object sender, EventArgs e)
        {

        }

        public void ShowImage(Bitmap pic)
        {
            pictureBox1.Image = pic;
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;
                double ratioX = (double)pictureBox1.Width / (double)pictureBox1.Image.Width;
                double ratioY = (double)pictureBox1.Height / (double)pictureBox1.Image.Height;
                // use whichever multiplier is smaller
                double ratio = ratioX < ratioY ? ratioX : ratioY;

                // now we can get the new height and width
                int newHeight = Convert.ToInt32(pictureBox1.Image.Height * ratio);
                int newWidth = Convert.ToInt32(pictureBox1.Image.Width * ratio);

                // Now calculate the X,Y position of the upper-left corner 
                // (one of these will always be zero)
                int posX = Convert.ToInt32((pictureBox1.Width - (pictureBox1.Image.Width * ratio)) / 2);
                int posY = Convert.ToInt32((pictureBox1.Height - (pictureBox1.Image.Height * ratio)) / 2);
                e.Graphics.Clear(pictureBox1.BackColor);
                e.Graphics.DrawImage(pictureBox1.Image, posX, posY, newWidth, newHeight);
            }
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Export";
            dlg.Filter = "bmp file (*.bmp)|*.bmp|jpg file (*.jpg)|*.jpg|png file (*.png)|*.png";
            dlg.AddExtension = true;
            //nothing here... :\
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                if (pictureBox1.Image != null)
                {
                    pictureBox1.Image.Save(dlg.FileName);
                }
            }
        }
    }
}
