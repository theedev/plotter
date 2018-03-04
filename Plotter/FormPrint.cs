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
    public partial class FormPrint : Form
    {
        public FormPrint()
        {
            InitializeComponent();
        }


        private Point MouseDownLocation;
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {

                MouseDownLocation = e.Location;
            }
        }

        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Left)
            {
                    pictureBox1.Left = e.X + pictureBox1.Left - MouseDownLocation.X;
                    pictureBox1.Top = e.Y + pictureBox1.Top - MouseDownLocation.Y;
                
            }
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void FormPrint_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = new Bitmap("C://Users/Farook/Pictures/Prog.png");
            pictureBox1.Size = new Size(pictureBox1.Image.Size.Width, pictureBox1.Image.Size.Height);
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (pictureBox1.Location.X + pictureBox1.Size.Width > panel1.Size.Width) {
                Point newlocc = new Point(panel1.Size.Width - pictureBox1.Size.Width, pictureBox1.Location.Y);
                pictureBox1.Location = newlocc;
            }

           if (pictureBox1.Location.Y + pictureBox1.Size.Height > panel1.Size.Height)
            {
                Point newlocy = new Point(pictureBox1.Location.X, panel1.Size.Height - pictureBox1.Size.Height);
                pictureBox1.Location = newlocy;
            }

            if (pictureBox1.Location.X + pictureBox1.Size.Width < panel1.Size.Width)
            {
                Point newlocc = new Point(0, pictureBox1.Location.Y);
                pictureBox1.Location = newlocc;
            }

            if (pictureBox1.Location.Y + pictureBox1.Size.Height < panel1.Size.Height)
            {
                Point newlocc = new Point(pictureBox1.Location.X, 0);
                pictureBox1.Location = newlocc;
            }
        }
    }

       
    }

