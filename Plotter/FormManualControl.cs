using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Plotter;

namespace Plotter
{
    public partial class FormManualControl : Form
    {
        Form1 frm1;
        SerialPort SP;
        int X = 0;
        int Y = 0;


        public FormManualControl(Form1 refer)
        {
            InitializeComponent();
            frm1 = refer;
            SP = new SerialPort(PlotterFunctions.comPort, PlotterFunctions.comRate);
            SP.Write("MCon;");
            SP.Close();
        }

        private void buttonUp_Click(object sender, EventArgs e)
        {
            if (Y > 0)
            {
                SP.Open();
                SP.Write("V0,-1;");
                Y--;
                SP.Close();
            }
        }

        private void buttonRight_Click(object sender, EventArgs e)
        {
            if (X < PlotterFunctions.pSizeX)
            {
                SP.Open();
                SP.Write("V1,0;");
                X++;
                SP.Close();
            }
        }

        private void buttonDown_Click(object sender, EventArgs e)
        {
            if (Y < PlotterFunctions.pSizeY)
            {
                SP.Open();
                SP.Write("V0,1;");
                Y++;
                SP.Close();
            }
        }

        private void buttonLeft_Click(object sender, EventArgs e)
        {
            if (X > 0)
            {
                SP.Open();
                SP.Write("V-1,0;");
                X--;
                SP.Close();
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            SP.Open();
            SP.Write("P;");
            SP.Close();
        }

        private void formManualControl_FormClosing(object sender, FormClosingEventArgs e)
        {
            SP.Open();
            SP.Write("MCoff;");
            SP.Close();
        }
    }
}
