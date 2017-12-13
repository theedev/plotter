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
        
        //a function that ovverrides windows' default interpolation mode in order to display a sharp image
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
            PlotterFunctions.loader(ConfigFileName);
            setNumerics();
            PlotterFunctions.reloadColours();
            PlotterFunctions.handshake();
            label4.Text = PlotterFunctions.plotterName;
            if (label4.Text.Equals ("Plotter Not Connected"))
                label4.ForeColor = Color.FromArgb(255, 0, 0);
            else
                label4.ForeColor = Color.FromArgb(0, 128, 32);
        }
        //in between void 
        void ditheronthread2 ()
        {
            Bitmap bmpint = (Bitmap)pictureBox1.Image;
            Bitmap bmpnew = PlotterFunctions.dither(bmpint, bmpint.Width, bmpint.Height, PlotterFunctions.compcol);
            pictureBox1.Image = bmpnew;
            PlotterFunctions.dithered = true;
        }
        //
        private void ditherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (pictureBox1.Image != null)
            {
                if (PlotterFunctions.compcol.Count > 1)
                {
                    Thread ditherThread = new Thread(new ThreadStart(ditheronthread2));
                    ditherThread.Start();
                }
                else
                {
                    MessageBox.Show("No Colors available", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("No picture loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        void generateonT2()
        {
            PlotterFunctions.colourMaps = PlotterFunctions.generateColourMaps((Bitmap)pictureBox1.Image, PlotterFunctions.compcol);
        }

        private void generateColourMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PlotterFunctions.dithered)
            {
                Thread Th2 = new Thread(new ThreadStart(generateonT2));
                Th2.Start();
            }
            else
                MessageBox.Show("Picture not dithered", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        void generatepatsonT2 ()
        {
            PlotterFunctions.patternMaps = PlotterFunctions.generatePatternMaps(PlotterFunctions.colourMaps);
        }

        private void generatePatternMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PlotterFunctions.colourMaps != null)
            {
                Thread Th2 = new Thread(new ThreadStart(generatepatsonT2));
                Th2.Start();
            }
            else
                MessageBox.Show("Colour maps not generated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        //in between voids
        
        void generateSeqOnthread()
        {
            FormLoadingcs loader = new FormLoadingcs(true, "Generating Pattern Sequences");
            loader.Show();

            Thread generatorThread = new Thread(actualGenerate,128*1024*1024);
                generatorThread.Start();
                while (generatorThread.ThreadState == ThreadState.Running)
                {
                    Application.DoEvents();
                }
            Thread.Sleep(100);
            loader.Close();
        }

        private void actualGenerate()
        {
            
            
            PlotterFunctions.outlineSequences = new List<List<Coordinate>>[(PlotterFunctions.patternMaps.Length / 2) - 1];
            PlotterFunctions.outlineSequences = PlotterFunctions.generateOutlineSequences(PlotterFunctions.patternMaps, this);
            PlotterFunctions.fillingSequences = new List<List<Coordinate>>[(PlotterFunctions.patternMaps.Length / 2) - 1];
            PlotterFunctions.fillingSequences = PlotterFunctions.generateFillingSequences(PlotterFunctions.patternMaps);

        }

        //
        private void generatePatternSequencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PlotterFunctions.patternMaps != null)
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
            if (PlotterFunctions.openPort())
            {
                PlotterFunctions.SP.Write("PMode;");
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
                PlotterFunctions.dithered = false;
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


        private void showColourMapsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (PlotterFunctions.colourMaps != null)
            {
                int count = 0;
                foreach (Bitmap colmap in PlotterFunctions.colourMaps)
                {
                    FormColMap frmclmap = new FormColMap();
                    if (PlotterFunctions.coloursInv.ContainsKey(PlotterFunctions.compcol[count]))
                        frmclmap.Text = PlotterFunctions.coloursInv[PlotterFunctions.compcol[count]];
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


        private void previewEdgeAndFillingMapsToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (PlotterFunctions.patternMaps != null)
            {
                for (int i = 0; i < ((PlotterFunctions.patternMaps.Length / 2)); i++)
                {
                    FormColMap frmclmap = new FormColMap();
                    FormColMap frmclmap2 = new FormColMap();
                    frmclmap.Text = PlotterFunctions.coloursInv[PlotterFunctions.compcol[i]] + " Outline";
                    frmclmap2.Text = PlotterFunctions.coloursInv[PlotterFunctions.compcol[i]] + " Filling";
                    frmclmap.ShowImage(PlotterFunctions.patternMaps[i, 0]);
                    frmclmap2.ShowImage(PlotterFunctions.patternMaps[i, 1]);
                    frmclmap.Show();
                    frmclmap2.Show();
                }
            }
            else
                MessageBox.Show("Pattern maps not generated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void manualControlModeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (PlotterFunctions.openPort())
            {
                FormManualControl MC = new FormManualControl(this);
                MC.Show();
            }
        }

        private void connectToPlotterToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            PlotterFunctions.handshake();
            label4.Text = PlotterFunctions.plotterName;
            if (label4.Text.Equals("Plotter Not Connected"))
                label4.ForeColor = Color.FromArgb(255, 0, 0);
            else
                label4.ForeColor = Color.FromArgb(0, 128, 32);
        }
        
        private void DiameterMain_ValueChanged(object sender, EventArgs e)
        {
            PlotterFunctions.diameter = PlotterFunctions.changeDiameter(DiameterMain.Value, DiameterDecimal.Value);
            if (!PlotterFunctions.settingNumerics)
                PlotterFunctions.saver(ConfigFileName);
        }

        private void DiameterDecimal_ValueChanged(object sender, EventArgs e)
        {
            PlotterFunctions.diameter = PlotterFunctions.changeDiameter(DiameterMain.Value, DiameterDecimal.Value);
            if (!PlotterFunctions.settingNumerics)
                PlotterFunctions.saver(ConfigFileName);
        }


        internal void setNumerics()
        {
            string[] splitfloat = new string[2];
            PlotterFunctions.settingNumerics = true;
            splitfloat = PlotterFunctions.diameter.ToString().Split('.');
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
            PlotterFunctions.settingNumerics = false;
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            PlotterFunctions.sendPrintingInfo(this);
        }


        private void halpToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StreamWriter sht = new StreamWriter("out.txt");
            for (int i = 0; i < PlotterFunctions.outlineSequences.Length; i++)
            {
                for (int j = 0; j < PlotterFunctions.outlineSequences[i].Count; j++)
                {
                    for (int k = 0; k < PlotterFunctions.outlineSequences[i][j].Count; k++)
                    {
                        sht.Write(PlotterFunctions.outlineSequences[i][j][k].X() + "," + PlotterFunctions.outlineSequences[i][j][k].Y() + ";");
                    }
                }
            }
            for (int i = 0; i < PlotterFunctions.fillingSequences.Length; i++)
            {
                for (int j = 0; j < PlotterFunctions.fillingSequences[i].Count; j++)
                {
                    for (int k = 0; k < PlotterFunctions.fillingSequences[i][j].Count; k++)
                    {
                        sht.Write(PlotterFunctions.fillingSequences[i][j][k].X() + "," + PlotterFunctions.fillingSequences[i][j][k].Y() + ";");
                    }
                }
            }
            sht.Close();
        }
    }
}