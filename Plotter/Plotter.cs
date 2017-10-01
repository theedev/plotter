using Plotter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;
using System.Windows.Forms;
using System.Threading;

public static class plotter
{
    //TODO protect the variables
    public const string ComPort = "COM3";
    public const int ComRate = 9600;
    internal static SerialPort SP = new SerialPort(ComPort, ComRate);

    //booleans to check what's going on
    internal static bool Dithered = false;
    internal static bool SettingNumerics = false;

    // add a dictionary that has a name and colour
    public static Dictionary<String, Color> Colours = new Dictionary<String, Color>();
    public static Dictionary<Color, String> Coloursinv = new Dictionary<Color, String>();

    //a list to compare colours
    internal static List<Color> compcol = new List<Color>();

    //the diameter of an ink dot
    internal static float Diameter = 0f;

    //an array of colour maps
    internal static Bitmap[] ColourMaps;
    //an array of pattern maps
    internal static Bitmap[,] PatternMaps;

    static List<Coordinate> IgnoredPixels;

    internal static List<List<Coordinate>>[] OutlineSequences;
    internal static List<List<Coordinate>>[] FillingSequences;

    static int currentH = 0;
    static int currentI = 0;
    static int currentJ = 0;
    static bool PrintingFilling = false;

    internal static bool PlotterAvailable = false;
    internal static string PlotterName = "Plotter Not Connected";
    internal static int PsizeX = 0;
    internal static int PsizeY = 0;
    internal static int DownAngle = 0;

    internal static int PatternProgressCount;


    internal static void Loader(String Filename)
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
                    line = file.ReadLine();
                }

            }
            file.Close();
        }

        else
        {
            CreateNewConfig(Filename);
            Loader(Filename);
        }

    }

    internal static void Saver(String Filename)
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

    internal static void ReloadCols()
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

    internal static void CreateNewConfig(string Filename)
    {
        StreamWriter file = new StreamWriter(Filename);
        file.WriteLine("#Colours");
        file.WriteLine("!;");
        file.WriteLine("#Diameter");
        file.WriteLine("0.00");
        file.WriteLine("!;");
        file.Close();
    }

    internal static float ChangeDiameter(decimal main, decimal deci)
    {
        float NewDiameter = 0f;
        NewDiameter += (float)main;
        NewDiameter += (float)deci / 100f;
        return NewDiameter;
    }

    


    internal static Bitmap dither(Bitmap src1, int width, int height, List<Color> Compcol)
    {
        Bitmap diffBM = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        FormLoadingcs frmld = new FormLoadingcs(false,"Dithering");
        frmld.Show();
        int progress = 0;
        if (Compcol.Count > 2)
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color col1 = src1.GetPixel(x, y);
                    Color newcol = Color.FromArgb(255, 255, 255);
                    int coldif = 256;

                    foreach (Color col in Compcol)
                    {

                        int r = 0, g = 0, b = 0;
                        r = Math.Abs(col1.R - col.R);
                        g = Math.Abs(col1.G - col.G);
                        b = Math.Abs(col1.B - col.B);

                        int dif = ((r + g + b) / 3);
                        if (dif < coldif)
                        {
                            coldif = dif;
                            newcol = col;
                        }
                    }
                    diffBM.SetPixel(x, y, newcol);


                }
                progress = (int)((float)y / (float)height * 100);
                frmld.setProgress(progress);
            }
        }
        else
        {
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Color col1 = src1.GetPixel(x, y);
                    Color newcol = Color.FromArgb(255, 255, 255);

                    foreach (Color col in Compcol)
                    {

                        int Avg = (col1.R + col1.G + col1.B) / 3;
                        if (Avg > 127)
                        {
                            newcol = Color.FromArgb(255, 255, 255);
                        }
                        else
                        {
                            newcol = Color.FromArgb(0, 0, 0);
                        }
                    }
                    diffBM.SetPixel(x, y, newcol);


                }
                progress = (int)((float)y / (float)height * 100);
                frmld.setProgress(progress);
            }
        }
        frmld.Close();
        return diffBM;
    }

    internal static Bitmap[] GenerateColourMaps(Bitmap pic, List<Color> cols)
    {
        Bitmap[] CMaps = new Bitmap[cols.Count];
        FormLoadingcs frmldgn = new FormLoadingcs();
        int count = -1;
        for (int i = 0; i < CMaps.Length; i++)
        {

            if (i < compcol.Count - 1)
                frmldgn.Text = "Generating - " + Coloursinv[compcol[i]] + " (" + (i + 1) + "/" + compcol.Count + ")";
            else
                frmldgn.Text = "Generating - White" + " (" + (i + 1) + "/" + compcol.Count + ")";

            frmldgn.Show();

            Bitmap colpic = new Bitmap(pic.Width, pic.Height, PixelFormat.Format24bppRgb);

            for (int y = 0; y < pic.Height; y++)
            {
                count++;
                for (int x = 0; x < pic.Width; x++)
                {
                    Color col1 = pic.GetPixel(x, y);
                    if (col1 == compcol[i])
                        colpic.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    else
                        colpic.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                }
                frmldgn.setProgress((int)(((float)count / (float)pic.Height * 100f) / compcol.Count));
            }
            CMaps[i] = colpic;

        }
        frmldgn.Close();
        return CMaps;
    }

    internal static Bitmap[,] GeneratePatternMaps(Bitmap[] ColMaps)
    {
        //create a 2 dimensional array
        Bitmap[,] Patterns = new Bitmap[ColMaps.Length - 1, 2];
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

    internal static Bitmap FindOutline(Color colour, Bitmap MaptoCheck, FormLoadingcs frmldgn)
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

    internal static void CheckPixel(Coordinate pixel, Color colour, Bitmap MapTC, Bitmap MapTA)
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

    internal static Bitmap FindFilling(Color colour, Bitmap OutlineMap, Bitmap MapToFill, FormLoadingcs frmldgn)
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

    internal static List<List<Coordinate>>[] GenerateOutlineSequences(Bitmap[,] patternMaps)
    {
        List<List<Coordinate>>[] OutlineSequenceImageList = new List<List<Coordinate>>[(patternMaps.Length / 2)];
        FormLoadingcs load = new FormLoadingcs(true, "Generating Outline Sequences");
        load.Show();
        
        //loop over colour outlines
        for (int i = 0; i < ((patternMaps.Length / 2)); i++)
        {
            load.setProgress(5);
            List<List<Coordinate>> SequenceList = new List<List<Coordinate>>();
            SequenceList = GenerateOutlineSequenceList(patternMaps[i, 0]);
            OutlineSequenceImageList[i] = SequenceList;
            
        }
        load.Close();
        return OutlineSequenceImageList;
    }

    internal static List<List<Coordinate>> GenerateOutlineSequenceList(Bitmap bitmap)
    {
        IgnoredPixels = new List<Coordinate>();
        int Width = bitmap.Width;
        int Height = bitmap.Height;
        List<List<Coordinate>> SL = new List<List<Coordinate>>();
        for (int j = 0; j < Height; j++)
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

    internal static void FindOutlineSequence(Coordinate pixel, Bitmap bitmap, List<Coordinate> sequence)
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
            if (newPixel.X() >= 0 && newPixel.X() < bitmap.Width && newPixel.Y() >= 0 && newPixel.Y() < bitmap.Height)
                if (bitmap.GetPixel(newPixel.X(), newPixel.Y()) == Color.FromArgb(255, 0, 0))
                {
                    FindOutlineSequence(newPixel, bitmap, sequence);
                }
        }
    }

    internal static void SendPrintingInfo(Form1 frm1)
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
                frm1.timer1.Enabled = false;
                currentH++;
                PrintingFilling = true;
                currentH--;
                currentI = 0;
                currentJ = 0;
                frm1.timer1.Enabled = true;
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
                frm1.timer1.Enabled = false;
                currentH++;
                if (currentH >= FillingSequences.Length && PrintingFilling)
                {
                    currentH = 0;
                    currentI = 0;
                    currentJ = 0;
                    frm1.timer1.Enabled = false;
                    SP.Close();
                    MessageBox.Show("The printing proccess is done.", "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    DialogResult dlgr = MessageBox.Show("Insert pen with colour " + plotter.Coloursinv[plotter.compcol[currentH]], "Notice", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    if (dlgr == DialogResult.OK)
                    {
                        currentI = 0;
                        currentJ = 0;
                        PrintingFilling = false;
                        frm1.timer1.Enabled = true;
                    }
                }
            }
        }
    }

    internal static List<List<Coordinate>>[] GenerateFillingSequences(Bitmap[,] patternMaps)
    {
        List<List<Coordinate>>[] FillingSequenceImageList = new List<List<Coordinate>>[(patternMaps.Length / 2)];
        FormLoadingcs load = new FormLoadingcs(true, "Generating Filling Sequences");
        load.Show();
        //loop over colour outlines
        for (int i = 0; i < ((patternMaps.Length / 2)); i++)
        {
            List<List<Coordinate>> SequenceList = new List<List<Coordinate>>();
            SequenceList = GenerateFillingSequenceList(patternMaps[i, 1]);
            FillingSequenceImageList[i] = SequenceList;
        }
        load.Close();
        return FillingSequenceImageList;
    }

    internal static List<List<Coordinate>> GenerateFillingSequenceList(Bitmap bitmap)
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

    internal static void FindFillingSequence(Coordinate pixel, Bitmap bitmap, List<Coordinate> sequence)
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
            //if any of them is green
            if (pixel.X() - 1 >= 0 && pixel.X() + 1 < bitmap.Width)
                if (bitmap.GetPixel(newPixel.X(), newPixel.Y()) == Color.FromArgb(0, 255, 0))
                {
                    FindOutlineSequence(newPixel, bitmap, sequence);
                }
        }
    }

    internal static void Handshake()
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
                PlotterName = HSI[0];
                PsizeX = Int32.Parse(HSI[1]);
                PsizeY = Int32.Parse(HSI[2]);
                DownAngle = Int32.Parse(HSI[3]);
            }


            SP.Close();
        }
    }

    internal static bool OpenPort()
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

}
