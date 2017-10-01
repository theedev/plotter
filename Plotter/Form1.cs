using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Plotter
{
    public partial class Form1 : Form
    {
        
        const string ConfigFileName = "Config.ini";
        
        
        public Form1()
        {
            InitializeComponent();
            pictureBox1.AllowDrop = true;
        }
        

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (pictureBox1.Image != null)
            {
                e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
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

        private void pictureBox1_DragDrop(object sender, DragEventArgs e)
        {
            var validExtensions = new[] { ".png", ".jpg", ".plt", ".bmp" };
            var lst = (IEnumerable<string>)e.Data.GetData(DataFormats.FileDrop);
            foreach (var ext in lst.Select((f) => System.IO.Path.GetExtension(f)))
            {
                if (validExtensions.Contains(ext))
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    pictureBox1.Image = Image.FromFile(files[0]);
                }
            }



        }

        private void pictureBox1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }



        private void Form1_Load(object sender, EventArgs e)
        {
            plotter.Loader(ConfigFileName);
            SetNumerics();
            plotter.ReloadCols();
            plotter.Handshake();
            label4.Text = plotter.PlotterName;
        }
        //in between void 
        void ditheronthread2 ()
        {
            Bitmap bmpint = (Bitmap)pictureBox1.Image;
            Bitmap bmpnew = plotter.dither(bmpint, bmpint.Width, bmpint.Height, plotter.compcol);
            pictureBox1.Image = bmpnew;
            plotter.Dithered = true;
        }
        //
        private void ditherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (pictureBox1.Image != null)
            {
                Thread ditherThread = new Thread(new ThreadStart(ditheronthread2));
                ditherThread.Start();
            }
            else
            {
                MessageBox.Show("No picture loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void generateColourMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (plotter.Dithered)
            {
                plotter.ColourMaps = plotter.GenerateColourMaps((Bitmap)pictureBox1.Image, plotter.compcol);
            }
            else
                MessageBox.Show("Picture not dithered", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void generatePatternMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (plotter.ColourMaps != null)
            {
                plotter.PatternMaps = plotter.GeneratePatternMaps(plotter.ColourMaps);
            }
            else
                MessageBox.Show("Colour maps not generated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        //in between void
        void generateSeqOnthread()
        {
            
            FormLoadingcs loader = new FormLoadingcs(true, "Generating Pattern Sequences");
            loader.Show();
                Thread generatorThread = new Thread(ActualGenerate,128*1024*1024);
                generatorThread.Start();
                while (generatorThread.ThreadState == ThreadState.Running)
                {
                    Application.DoEvents();
                }
                Thread.Sleep(1000);
            loader.Close();
        }

        private void ActualGenerate()
        {
            plotter.OutlineSequences = new List<List<Coordinate>>[(plotter.PatternMaps.Length / 2) - 1];
            plotter.OutlineSequences = plotter.GenerateOutlineSequences(plotter.PatternMaps);
            plotter.FillingSequences = new List<List<Coordinate>>[(plotter.PatternMaps.Length / 2) - 1];
            plotter.FillingSequences = plotter.GenerateFillingSequences(plotter.PatternMaps);
        }

        //
        private void generatePatternSequencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (plotter.PatternMaps != null)
            {
                const int Size = 128 * 1024 * 1024;
                Thread generateThread = new Thread(new ThreadStart(generateSeqOnthread), Size);
                generateThread.Start();
                
            }
            else
                MessageBox.Show("Pattern maps not generated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void printOutlinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (plotter.OpenPort())
            {
                plotter.SP.Write("PMode;");
                timer1.Enabled = true;
                //SendOutlineInfo();
            }
        }

        private void colourListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormColour frmclr = new FormColour();
            frmclr.Show();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog
            {
                Title = "Open Image",
                Filter = "Plot file (*.plt)|*.plt|bmp files (*.bmp)|*.bmp|jpg files (*.jpg)|*.jpg|png files (*.png)|*.png"
            };
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(dlg.FileName);
                plotter.Dithered = false;
            }
        }

        private void saveImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Title = "Save Plot";
            dlg.Filter = "Plot file (*.plt)|*.plt|bmp file (*.bmp)|*.bmp|jpg file (*.jpg)|*.jpg|png file (*.png)|*.png";
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

        private void showColourMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void showColourMapsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (plotter.ColourMaps != null)
            {
                int count = 0;
                foreach (Bitmap colmap in plotter.ColourMaps)
                {
                    FormColMap frmclmap = new FormColMap();
                    if (plotter.Coloursinv.ContainsKey(plotter.compcol[count]))
                        frmclmap.Text = plotter.Coloursinv[plotter.compcol[count]];
                    else
                        frmclmap.Text = "White";
                    frmclmap.ShowImage(colmap);
                    frmclmap.Show();
                    count++;
                }
            }
            else
                MessageBox.Show("Colour maps not generated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void previewEdgeAndFillingMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void previewEdgeAndFillingMapsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (plotter.PatternMaps != null)
            {
                for (int i = 0; i < ((plotter.PatternMaps.Length / 2) - 1); i++)
                {
                    FormColMap frmclmap = new FormColMap();
                    FormColMap frmclmap2 = new FormColMap();
                    frmclmap.Text = plotter.Coloursinv[plotter.compcol[i]] + " Outline";
                    frmclmap2.Text = plotter.Coloursinv[plotter.compcol[i]] + " Filling";
                    frmclmap.ShowImage(plotter.PatternMaps[i, 0]);
                    frmclmap2.ShowImage(plotter.PatternMaps[i, 1]);
                    frmclmap.Show();
                    frmclmap2.Show();
                }
            }
            else
                MessageBox.Show("Pattern maps not generated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void manualControlModeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (plotter.OpenPort())
            {
                FormManualControl MC = new FormManualControl(this);
                MC.Show();
            }
        }

        private void connectToPlotterToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            plotter.Handshake();
            label4.Text = plotter.PlotterName;
        }
        
        private void DiameterMain_ValueChanged(object sender, EventArgs e)
        {
            plotter.Diameter = plotter.ChangeDiameter(DiameterMain.Value, DiameterDecimal.Value);
            if (!plotter.SettingNumerics)
                plotter.Saver(ConfigFileName);
        }

        private void DiameterDecimal_ValueChanged(object sender, EventArgs e)
        {
            plotter.Diameter = plotter.ChangeDiameter(DiameterMain.Value, DiameterDecimal.Value);
            if (!plotter.SettingNumerics)
                plotter.Saver(ConfigFileName);
        }


        internal void SetNumerics()
        {
            string[] splitfloat = new string[2];
            plotter.SettingNumerics = true;
            splitfloat = plotter.Diameter.ToString().Split('.');
            DiameterMain.Value = decimal.Parse(splitfloat[0]);
            if (splitfloat.Length > 1)
            {
                if (!splitfloat[1][0].Equals('0'))
                {
                    if (float.Parse(splitfloat[1] + "0") < 100f)
                    {
                        DiameterDecimal.Value = decimal.Parse(splitfloat[1]) * 10;
                    }
                    else
                        DiameterDecimal.Value = decimal.Parse(splitfloat[1]);
                }
                else
                    DiameterDecimal.Value = decimal.Parse(splitfloat[1]);
            }
            else
                DiameterDecimal.Value = 0;
            plotter.SettingNumerics = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            plotter.SendPrintingInfo(this);
        }
    }
}
/*
        private void SendOutlineInfo()
        {
            if (PlotterAvailable)
            {
                SP.Open();
                SP.Write("PMode;");
                for (int h = 0; h < compcol.Count - 1; h++)
                {
                    if (MessageBox.Show("Please insert pen with colour " + Coloursinv[compcol[h]], "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information) = DialogResult.OK)
                    for (int i = 0; i < Sequences[0].Count; i++)
                    {
                        for (int j = 0; j < Sequences[0][i].Count; j++)
                        {
                            char[] bytes = new char[4];
                            int X = Sequences[0][i][j].X();
                            int Y = Sequences[0][i][j].Y();

                            bytes[3] = (char)((Y >> 8) & 0xFF);
                            bytes[2] = (char)(Y & 0xFF);
                            bytes[1] = (char)((X >> 8) & 0xFF);
                            bytes[0] = (char)(X & 0xFF);

                            string k = new string(bytes);

                            SP.Write(k);

                            //SP.Write(Sequences[0][i][j].X() + "," + Sequences[0][i][j].Y() + ";");
                            //textBox1.Text += Sequences[0][i][j].X() + "," + Sequences[0][i][j].Y() + ";";
                        }
                    }
                }
                //SP.Write(":");
                SP.Close();
            }
        }
        */