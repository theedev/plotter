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
using Plotter;

namespace Plotter
{
    public partial class Form1 : Form
    {
        
        //
        //
        //Variable Declarations 
        //
        //

        // add a dictionary that has a name and colour
        public Dictionary<String, Color> Colours = new Dictionary<String, Color>();
        public Dictionary<Color, String> Coloursinv = new Dictionary<Color, String>();

        //a list to compare colours
        internal List<Color> compcol = new List<Color>();

        //booleans to check what's going on
        bool Dithered = false;
        bool SettingNumerics = false;

        //an array of colour maps
        Bitmap[] ColourMaps;
        //an array of pattern maps
        Bitmap[,] PatternMaps;

        //the diameter of an ink dot
        float Diameter = 0f;

        bool PlotterAvailable = false;

        int PatternProgressCount;

        List<List<Coordinate>>[] OutlineSequences;
        List<List<Coordinate>>[] FillingSequences;
        List<Coordinate> IgnoredPixels;

        const string ConfigFileName = "Config.ini";

        public const string ComPort = "COM3";
        public const int ComRate = 9600;
        SerialPort SP = new SerialPort(ComPort, ComRate);
        internal int PsizeX = 0;
        internal int PsizeY = 0;
        internal int DownAngle = 0;

        int currentH = 0;
        int currentI = 0;
        int currentJ = 0;
        bool PrintingFilling = false;

        //
        //
        //Variable Declarations
        //
        //
    
        public Form1()
        {
            InitializeComponent();
            pictureBox1.AllowDrop = true;
            Loader(ConfigFileName);
            ReloadCols();
            Handshake();
        }

        //
        //
        //Structural Methods
        //
        //

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

        }

        //
        //
        //Button Methods
        //
        //

        private void ditherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bmpint = (Bitmap)pictureBox1.Image;
            if (pictureBox1.Image != null)
            {
                Bitmap bmpnew = plotter.dither(bmpint, bmpint.Width, bmpint.Height, compcol);
                pictureBox1.Image = bmpnew;
                Dithered = true;
            }
            else
            {
                MessageBox.Show("No picture loaded", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void generateColourMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Dithered)
            {
                ColourMaps = plotter.GenerateColourMaps((Bitmap)pictureBox1.Image, compcol, this);
            }
            else
                MessageBox.Show("Picture not dithered", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void generatePatternMapsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ColourMaps != null)
            {
                PatternMaps = GeneratePatternMaps(ColourMaps);
            }
            else
                MessageBox.Show("Colour maps not generated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void generatePatternSequencesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (PatternMaps != null)
            {
                OutlineSequences = new List<List<Coordinate>>[(PatternMaps.Length / 2) - 1];
                OutlineSequences = GenerateOutlineSequences(PatternMaps);
                FillingSequences = new List<List<Coordinate>>[(PatternMaps.Length / 2) - 1];
                FillingSequences = GenerateFillingSequences(PatternMaps);
            }
            else
                MessageBox.Show("Pattern maps not generated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void printOutlinesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (OpenPort())
            {
                SP.Write("PMode;");
                timer1.Enabled = true;
                //SendOutlineInfo();
            }
        }

        private void colourListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FormColour frmclr = new FormColour(this);
            frmclr.Show();
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void openImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Title = "Open Image";
            dlg.Filter = "Plot file (*.plt)|*.plt|bmp files (*.bmp)|*.bmp|jpg files (*.jpg)|*.jpg|png files (*.png)|*.png";
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pictureBox1.Image = new Bitmap(dlg.FileName);
                Dithered = false;
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
            if (ColourMaps != null)
            {
                int count = 0;
                foreach (Bitmap colmap in ColourMaps)
                {
                    FormColMap frmclmap = new FormColMap();
                    if (Coloursinv.ContainsKey(compcol[count]))
                        frmclmap.Text = Coloursinv[compcol[count]];
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
            if (PatternMaps != null)
            {
                for (int i = 0; i < ((PatternMaps.Length / 2) - 1); i++)
                {
                    FormColMap frmclmap = new FormColMap();
                    FormColMap frmclmap2 = new FormColMap();
                    frmclmap.Text = Coloursinv[compcol[i]] + " Outline";
                    frmclmap2.Text = Coloursinv[compcol[i]] + " Filling";
                    frmclmap.ShowImage(PatternMaps[i, 0]);
                    frmclmap2.ShowImage(PatternMaps[i, 1]);
                    frmclmap.Show();
                    frmclmap2.Show();
                }
            }
            else
                MessageBox.Show("Pattern maps not generated", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void manualControlModeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (OpenPort())
            {
                FormManualControl MC = new FormManualControl(this);
                MC.Show();
            }
        }

        private void connectToPlotterToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Handshake();
        }


        private void DiameterMain_ValueChanged(object sender, EventArgs e)
        {
            Diameter = ChangeDiameter();
            if (!SettingNumerics)
                Saver(ConfigFileName);
        }

        private void DiameterDecimal_ValueChanged(object sender, EventArgs e)
        {
            Diameter = ChangeDiameter();
            if (!SettingNumerics)
                Saver(ConfigFileName);
        }

        //
        //
        //Functional Methods
        //
        //

        
        internal void Loader(String Filename)
        {
            //Access the file
            string line;
            StreamReader file = null;
            if (File.Exists(Filename))
            {
                file = new StreamReader(Filename);
                //Loop load all the data (colours for now) into the dictionary
                while ((line = file.ReadLine()) != null)
                {
                    if (line.Equals("#Colours"))
                    {
                        Colours.Clear();
                        line = file.ReadLine();
                        while (!line.Equals("!;"))
                        {
                            String[] colourPrep = line.Split(':');
                            Color ColourTemp = Color.FromArgb(Int32.Parse(colourPrep[1]), Int32.Parse(colourPrep[2]), Int32.Parse(colourPrep[3]));
                            Colours.Add(colourPrep[0], ColourTemp);
                            line = file.ReadLine();
                        }
                    }
                    if (line.Equals("#Diameter"))
                    {
                        line = file.ReadLine();
                        Diameter = float.Parse(line);
                        SetNumerics();
                        line = file.ReadLine();
                    }

                }
                file.Close();
            }

            else
            {
                CreateNewConfig(ConfigFileName);
                Loader(ConfigFileName);
            }

        }

        internal void Saver(String Filename)
        {
            StreamWriter file = new StreamWriter(Filename);
            //looping colours
            file.WriteLine("#Colours");
            foreach (KeyValuePair<String, Color> colour in Colours)
            {
                file.WriteLine(colour.Key + ":" + colour.Value.R + ":" + colour.Value.G + ":" + colour.Value.B);
            }
            file.WriteLine("!;");
            file.WriteLine("#Diameter");
            file.Write(Diameter);
            if (Diameter.ToString().Split('.').Length > 1)
            {
                file.WriteLine("0");
            }
            else
            {
                file.WriteLine(".0");
            }
            file.WriteLine("!;");
            file.Close();
        }

        internal void ReloadCols()
        {
            compcol.Clear();
            foreach (KeyValuePair<String, Color> colour in Colours)
            {
                compcol.Add(colour.Value);
            }
            compcol.Add(Color.FromArgb(255, 255, 255));
            Coloursinv.Clear();
            foreach (KeyValuePair<String, Color> colToInv in Colours)
            {
                Coloursinv.Add(colToInv.Value, colToInv.Key);
            }
        }

        internal void CreateNewConfig(string Filename)
        {
            StreamWriter file = new StreamWriter(Filename);
            file.WriteLine("#Colours");
            file.WriteLine("!;");
            file.WriteLine("#Diameter");
            file.WriteLine("0.00");
            file.WriteLine("!;");
            file.Close();
        }

        internal float ChangeDiameter()
        {
            float NewDiameter = 0f;
            NewDiameter += (float)DiameterMain.Value;
            NewDiameter += (float)DiameterDecimal.Value / 100f;
            return NewDiameter;
        }

        internal void SetNumerics()
        {
            string[] splitfloat = new string[2];
            SettingNumerics = true;
            splitfloat = Diameter.ToString().Split('.');
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
            SettingNumerics = false;
        }



        internal Bitmap[,] GeneratePatternMaps(Bitmap[] ColMaps)
        {
            //create a 2 dimensional array
            Bitmap[,] Patterns = new Bitmap[ColMaps.Length-1, 2];
            //take colour image
            FormLoadingcs frmldgn = new FormLoadingcs();
            PatternProgressCount = -1;
            frmldgn.Show();
            int counter = 0;
            for (int i = 0; i < ColMaps.Length - 1; i++)
            {
                counter++;
                Bitmap bmp = ColMaps[i];
                //create 2 bitmaps, one for the outline and the other for the filling
                Bitmap Outline = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
                Bitmap Filling = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);

                if (i < compcol.Count - 1)
                    frmldgn.Text = "Generating Outline - " + Coloursinv[compcol[i]] + " (" + (counter) + "/" + ((compcol.Count - 1) * 2) + ")";

                //call an outlining method on the first bitmap, specify colour
                Outline = FindOutline(Color.FromArgb(255, 0, 0), bmp, frmldgn);
                //call a filling method on the second bitmap, specify colour
                counter++;
                if (i < compcol.Count - 1)
                    frmldgn.Text = "Generating Filling - " + Coloursinv[compcol[i]] + " (" + (counter) + "/" + ((compcol.Count - 1) * 2) + ")";

                Filling = FindFilling(Color.FromArgb(0, 255, 0), Outline, bmp, frmldgn);
                //add the 2 bitmaps to the Array
                Patterns[i, 0] = Outline;
                Patterns[i, 1] = Filling;
            }
            frmldgn.Close();
            return Patterns;
        }

        internal Bitmap FindOutline(Color colour, Bitmap MaptoCheck, FormLoadingcs frmldgn)
        {
            //create a list of coordinates to ignore
            Bitmap linmap = new Bitmap(MaptoCheck.Width, MaptoCheck.Height, PixelFormat.Format24bppRgb);
            //loop though colour map
            for (int y = 0; y < MaptoCheck.Height; y++)
            {
                PatternProgressCount++;
                for (int x = 0; x < MaptoCheck.Width; x++)
                {
                    Coordinate pixel = new Coordinate(x, y);
                    //if the pixel is a 1 (black) then check pixel
                    if (MaptoCheck.GetPixel(x, y) == Color.FromArgb(0, 0, 0))
                    {
                        CheckPixel(pixel, colour, MaptoCheck, linmap);
                    }
                }
                frmldgn.setProgress((int)(((float)PatternProgressCount / (float)MaptoCheck.Height * 100f) / (float)((compcol.Count - 1) * 2)));
            }
            return linmap;
        }

        internal void CheckPixel(Coordinate pixel, Color colour, Bitmap MapTC, Bitmap MapTA)
        {
            Coordinate[] SurroundersDirect =
           {
                new Coordinate(pixel.X()-1,pixel.Y()),
                new Coordinate(pixel.X()+1,pixel.Y()),
                new Coordinate(pixel.X(),pixel.Y()-1),
                new Coordinate(pixel.X(),pixel.Y()+1),
            };
            //check the 4 direct pixels surrounding the pixel(up down left right)
            foreach (Coordinate coord in SurroundersDirect)
            {

                //if any of them is white then turn the pixel red and keep checking
                if (coord.X() > 0 && coord.X() < MapTC.Width && coord.Y() > 0 && coord.Y() < MapTC.Height)
                {
                    if (MapTC.GetPixel(coord.X(), coord.Y()) == Color.FromArgb(255, 255, 255))
                    {
                        MapTA.SetPixel(pixel.X(), pixel.Y(), Color.FromArgb(255, 0, 0));
                    }
                }
            }
        }

        internal Bitmap FindFilling(Color colour, Bitmap OutlineMap, Bitmap MapToFill, FormLoadingcs frmldgn)
        {
            Bitmap FillingMap = new Bitmap(OutlineMap.Width, OutlineMap.Height, PixelFormat.Format24bppRgb);
            for (int y = 0; y < OutlineMap.Height; y++)
            {
                PatternProgressCount++;
                for (int x = 0; x < OutlineMap.Width; x++)
                {
                    if (MapToFill.GetPixel(x, y) == Color.FromArgb(0, 0, 0))
                    {
                        if (OutlineMap.GetPixel(x, y) != Color.FromArgb(255, 0, 0))
                        {
                            FillingMap.SetPixel(x, y, colour);
                        }
                    }
                }
                frmldgn.setProgress((int)(((float)PatternProgressCount / (float)MapToFill.Height * 100f) / (float)((compcol.Count - 1) * 2)));
            }
            return FillingMap;
        }

        internal List<List<Coordinate>>[] GenerateOutlineSequences(Bitmap[,] patternMaps)
        {
            List<List<Coordinate>>[] OutlineSequenceImageList = new List<List<Coordinate>>[(patternMaps.Length/2)];
            //loop over colour outlines
            for (int i = 0; i < ((patternMaps.Length/2)); i++)
            {   
                List<List<Coordinate>> SequenceList = new List<List<Coordinate>>();
                SequenceList = GenerateOutlineSequenceList(patternMaps[i,0]);
                OutlineSequenceImageList[i] = SequenceList;
            }

            return OutlineSequenceImageList;
        }

        internal List<List<Coordinate>> GenerateOutlineSequenceList(Bitmap bitmap)
        {
            IgnoredPixels = new List<Coordinate>();
            int Width = bitmap.Width;
            int Height = bitmap.Height;
            List<List<Coordinate>> SL = new List<List<Coordinate>>();
            for (int j = 0; j<Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    if (bitmap.GetPixel(i, j) == Color.FromArgb(255, 0, 0))
                    {
                        bool Ignored = false;
                        Coordinate pixel = new Coordinate(i, j);
                        foreach (Coordinate pixelToCompare in IgnoredPixels)
                        {
                            if (pixel.Compare(pixelToCompare))
                            {
                                Ignored = true;
                                break;
                            }
                            else
                                Ignored = false;
                        }
                        if (!Ignored)
                        {
                          
                            List<Coordinate> pattern = new List<Coordinate>();
                            FindOutlineSequence(pixel, bitmap, pattern);
                            SL.Add(pattern);
                        }
                    }
                }
            }
            return SL;
        }

        internal void FindOutlineSequence(Coordinate pixel, Bitmap bitmap, List<Coordinate> sequence)
        {
            bool Ignored = false;

            foreach (Coordinate pixelToCompare in IgnoredPixels)
            {
                if (pixel.Compare(pixelToCompare))
                {
                    Ignored = true;
                    break;
                }
            }
            if (Ignored) return;
            sequence.Add(pixel);
            IgnoredPixels.Add(pixel);

            Coordinate[] SurroundingPixels =
            {
                new Coordinate(pixel.X()-1, pixel.Y()),
                new Coordinate(pixel.X()+1, pixel.Y()),
                new Coordinate(pixel.X(), pixel.Y()-1),
                new Coordinate(pixel.X(), pixel.Y()+1),
                new Coordinate(pixel.X()-1, pixel.Y()-1),
                new Coordinate(pixel.X()+1, pixel.Y()+1),
                new Coordinate(pixel.X()-1, pixel.Y()+1),
                new Coordinate(pixel.X()+1, pixel.Y()-1),
            };
            //check the surrounding pixels of a pixel
            foreach (Coordinate newPixel in SurroundingPixels)
            {
                //if any of them is red
                if (newPixel.X() >= 0 && newPixel.X() <= bitmap.Width && newPixel.Y() >= 0 && newPixel.Y() <= bitmap.Height)
                    if (bitmap.GetPixel(newPixel.X(),newPixel.Y()) == Color.FromArgb(255,0,0))
                {
                    FindOutlineSequence(newPixel, bitmap, sequence);  
                }
            }
           
        }

        internal List<List<Coordinate>>[] GenerateFillingSequences(Bitmap[,] patternMaps)
        {
            List<List<Coordinate>>[] FillingSequenceImageList = new List<List<Coordinate>>[(patternMaps.Length / 2)];
            //loop over colour outlines
            for (int i = 0; i < ((patternMaps.Length / 2)); i++)
            {
                List<List<Coordinate>> SequenceList = new List<List<Coordinate>>();
                SequenceList = GenerateFillingSequenceList(patternMaps[i, 1]);
                FillingSequenceImageList[i] = SequenceList;
            }

            return FillingSequenceImageList;
        }

        internal List<List<Coordinate>> GenerateFillingSequenceList(Bitmap bitmap)
        {
            IgnoredPixels = new List<Coordinate>();
            int Width = bitmap.Width;
            int Height = bitmap.Height;
            List<List<Coordinate>> SL = new List<List<Coordinate>>();
            for (int j = 0; j < Height; j++)
            {
                for (int i = 0; i < Width; i++)
                {
                    if (bitmap.GetPixel(i, j) == Color.FromArgb(0, 255, 0))
                    {
                        bool Ignored = false;
                        Coordinate pixel = new Coordinate(i, j);
                        foreach (Coordinate pixelToCompare in IgnoredPixels)
                        {
                            if (pixel.Compare(pixelToCompare))
                            {
                                Ignored = true;
                                break;
                            }
                            else
                                Ignored = false;
                        }
                        if (!Ignored)
                        {

                            List<Coordinate> pattern = new List<Coordinate>();
                            FindFillingSequence(pixel, bitmap, pattern);
                            SL.Add(pattern);
                        }
                    }
                }
            }
            return SL;
        }

        internal void FindFillingSequence(Coordinate pixel, Bitmap bitmap, List<Coordinate> sequence)
        {

            bool Ignored = false;

            foreach (Coordinate pixelToCompare in IgnoredPixels)
            {
                if (pixel.Compare(pixelToCompare))
                {
                    Ignored = true;
                    break;
                }
            }
            if (Ignored) return;
            sequence.Add(pixel);
            IgnoredPixels.Add(pixel);

            Coordinate[] SurroundingPixels =
            {
                new Coordinate(pixel.X()+1, pixel.Y()),
            };
            //check the surrounding pixels of a pixel
            foreach (Coordinate newPixel in SurroundingPixels)
            {
                //if any of them is red
                if (pixel.X()-1 >= 0)
                    if (bitmap.GetPixel(newPixel.X(), newPixel.Y()) == Color.FromArgb(0, 255, 0))
                    {
                        FindOutlineSequence(newPixel, bitmap, sequence);
                    }
            }
        }

        internal void Handshake()
        {
            PlotterAvailable = false;
            String[] HSI = new String[4];
            String HandshakeInfo;
            if (OpenPort())
            {
                //Send a handshake request
                
                SP.Write("OK;");
                HandshakeInfo = SP.ReadLine();
                if (HandshakeInfo == "OKAYYYYYYY\r")
                    PlotterAvailable = true;
                if (PlotterAvailable)
                {
                    //Request a plotter's name
                    SP.Write("RName;");
                    HandshakeInfo = SP.ReadLine();
                    HSI[0] = HandshakeInfo;
                    //Request a plotter's working area dimentions
                    SP.Write("RSizeX;");
                    HandshakeInfo = SP.ReadLine();
                    HSI[1] = HandshakeInfo;
                    SP.Write("RSizeY;");
                    HandshakeInfo = SP.ReadLine();
                    HSI[2] = HandshakeInfo;
                    //Request the angle at which the pen touches the paper or floor
                    SP.Write("RDownAngle;");
                    HandshakeInfo = SP.ReadLine();
                    HSI[3] = HandshakeInfo;

                    //dumping all the info from the string
                    label4.Text = HSI[0];
                    PsizeX = Int32.Parse(HSI[1]);
                    PsizeY = Int32.Parse(HSI[2]);
                    DownAngle = Int32.Parse(HSI[3]);
                }

                
                SP.Close();
            }
        }

        internal bool OpenPort()
        {
            try
            {
                SP.Open();
            }
            catch
            {
                MessageBox.Show("Device not connected", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            return true;

        }

        private void timer1_Tick(object sender, EventArgs e)
        {

            if (!PrintingFilling)
            {
                uint X = (uint)OutlineSequences[currentH][currentI][currentJ].X();
                uint Y = (uint)OutlineSequences[currentH][currentI][currentJ].Y();
                char[] bytes = new char[4];
                bytes[3] = (char)((Y >> 8) & 0xFF);
                bytes[2] = (char)(Y & 0xFF);
                bytes[1] = (char)((X >> 8) & 0xFF);
                bytes[0] = (char)(X & 0xFF);


                string BytesCombined = new string(bytes);

                SP.Write(BytesCombined);

                currentJ++;

                if (currentJ >= OutlineSequences[currentH][currentI].Count)
                {
                    currentJ = 0;
                    currentI++;

                }
                if (currentI >= OutlineSequences[currentH].Count)
                {
                    timer1.Enabled = false;
                    currentH++;
                    PrintingFilling = true;
                    currentH--;
                    currentI = 0;
                    currentJ = 0;
                    timer1.Enabled = true;
                }
            }
            else
            {
                uint X = (uint)FillingSequences[currentH][currentI][currentJ].X();
                uint Y = (uint)FillingSequences[currentH][currentI][currentJ].Y();


                char[] bytes = new char[4];
                bytes[3] = (char)((Y >> 8) & 0xFF);
                bytes[2] = (char)(Y & 0xFF);
                bytes[1] = (char)((X >> 8) & 0xFF);
                bytes[0] = (char)(X & 0xFF);


                string BytesCombined = new string(bytes);

                SP.Write(BytesCombined);

                currentJ++;

                if (currentJ >= FillingSequences[currentH][currentI].Count)
                {
                    currentJ = 0;
                    currentI++;

                }
                if (currentI >= FillingSequences[currentH].Count)
                {
                    timer1.Enabled = false;
                    currentH++;
                    if (currentH >= FillingSequences.Length && PrintingFilling)
                    {
                        currentH = 0;
                        currentI = 0;
                        currentJ = 0;
                        timer1.Enabled = false;
                        SP.Close();
                        MessageBox.Show("The printing proccess is done.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        DialogResult dlgr = MessageBox.Show("Insert pen with colour " + Coloursinv[compcol[currentH]], "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        if (dlgr == DialogResult.OK)
                        {
                            currentI = 0;
                            currentJ = 0;
                            PrintingFilling = false;
                            timer1.Enabled = true;
                        }
                    }
                }
            }
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