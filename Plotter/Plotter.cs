using Plotter;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.IO.Ports;

public static class plotter
{
    static List<Coordinate> IgnoredPixels;


    internal static Bitmap dither(Bitmap src1, int width, int height, List<Color> Compcol)
    {
        Bitmap diffBM = new Bitmap(width, height, PixelFormat.Format24bppRgb);
        FormLoadingcs frmld = new FormLoadingcs();
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

    internal static Bitmap[] GenerateColourMaps(Bitmap pic, List<Color> cols, Form1 frm1)
    {
        Bitmap[] CMaps = new Bitmap[cols.Count];
        FormLoadingcs frmldgn = new FormLoadingcs();
        int count = -1;
        for (int i = 0; i < CMaps.Length; i++)
        {

            if (i < frm1.compcol.Count - 1)
                frmldgn.Text = "Generating - " + frm1.Coloursinv[frm1.compcol[i]] + " (" + (i + 1) + "/" + frm1.compcol.Count + ")";
            else
                frmldgn.Text = "Generating - White" + " (" + (i + 1) + "/" + frm1.compcol.Count + ")";

            frmldgn.Show();

            Bitmap colpic = new Bitmap(pic.Width, pic.Height, PixelFormat.Format24bppRgb);

            for (int y = 0; y < pic.Height; y++)
            {
                count++;
                for (int x = 0; x < pic.Width; x++)
                {
                    Color col1 = pic.GetPixel(x, y);
                    if (col1 == frm1.compcol[i])
                        colpic.SetPixel(x, y, Color.FromArgb(0, 0, 0));
                    else
                        colpic.SetPixel(x, y, Color.FromArgb(255, 255, 255));
                }
                frmldgn.setProgress((int)(((float)count / (float)pic.Height * 100f) / frm1.compcol.Count));
            }
            CMaps[i] = colpic;

        }
        frmldgn.Close();
        return CMaps;
    }

    internal static Bitmap[,] GeneratePatternMaps(Bitmap[] ColMaps, Form1 frm1)
    {
        //create a 2 dimensional array
        Bitmap[,] Patterns = new Bitmap[ColMaps.Length - 1, 2];
        //take colour image
        FormLoadingcs frmldgn = new FormLoadingcs();
        frm1.PatternProgressCount = -1;
        frmldgn.Show();
        int counter = 0;
        for (int i = 0; i < ColMaps.Length - 1; i++)
        {
            counter++;
            Bitmap bmp = ColMaps[i];
            //create 2 bitmaps, one for the outline and the other for the filling
            Bitmap Outline = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);
            Bitmap Filling = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format24bppRgb);

            if (i < frm1.compcol.Count - 1)
                frmldgn.Text = "Generating Outline - " + frm1.Coloursinv[frm1.compcol[i]] + " (" + (counter) + "/" + ((frm1.compcol.Count - 1) * 2) + ")";

            //call an outlining method on the first bitmap, specify colour
            Outline = FindOutline(Color.FromArgb(255, 0, 0), bmp, frmldgn, frm1);
            //call a filling method on the second bitmap, specify colour
            counter++;
            if (i < frm1.compcol.Count - 1)
                frmldgn.Text = "Generating Filling - " + frm1.Coloursinv[frm1.compcol[i]] + " (" + (counter) + "/" + ((frm1.compcol.Count - 1) * 2) + ")";

            Filling = FindFilling(Color.FromArgb(0, 255, 0), Outline, bmp, frmldgn, frm1);
            //add the 2 bitmaps to the Array
            Patterns[i, 0] = Outline;
            Patterns[i, 1] = Filling;
        }
        frmldgn.Close();
        return Patterns;
    }

    internal static Bitmap FindOutline(Color colour, Bitmap MaptoCheck, FormLoadingcs frmldgn, Form1 frm1)
    {
        //create a list of coordinates to ignore
        Bitmap linmap = new Bitmap(MaptoCheck.Width, MaptoCheck.Height, PixelFormat.Format24bppRgb);
        //loop though colour map
        for (int y = 0; y < MaptoCheck.Height; y++)
        {
            frm1.PatternProgressCount++;
            for (int x = 0; x < MaptoCheck.Width; x++)
            {
                Coordinate pixel = new Coordinate(x, y);
                //if the pixel is a 1 (black) then check pixel
                if (MaptoCheck.GetPixel(x, y) == Color.FromArgb(0, 0, 0))
                {
                    CheckPixel(pixel, colour, MaptoCheck, linmap);
                }
            }
            frmldgn.setProgress((int)(((float)frm1.PatternProgressCount / (float)MaptoCheck.Height * 100f) / (float)((frm1.compcol.Count - 1) * 2)));
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

    internal static Bitmap FindFilling(Color colour, Bitmap OutlineMap, Bitmap MapToFill, FormLoadingcs frmldgn, Form1 frm1)
    {
        Bitmap FillingMap = new Bitmap(OutlineMap.Width, OutlineMap.Height, PixelFormat.Format24bppRgb);
        for (int y = 0; y < OutlineMap.Height; y++)
        {
            frm1.PatternProgressCount++;
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
            frmldgn.setProgress((int)(((float)frm1.PatternProgressCount / (float)MapToFill.Height * 100f) / (float)((frm1.compcol.Count - 1) * 2)));
        }
        return FillingMap;
    }

    internal static List<List<Coordinate>>[] GenerateOutlineSequences(Bitmap[,] patternMaps)
    {
        List<List<Coordinate>>[] OutlineSequenceImageList = new List<List<Coordinate>>[(patternMaps.Length / 2)];
        //loop over colour outlines
        for (int i = 0; i < ((patternMaps.Length / 2)); i++)
        {
            List<List<Coordinate>> SequenceList = new List<List<Coordinate>>();
            SequenceList = GenerateOutlineSequenceList(patternMaps[i, 0]);
            OutlineSequenceImageList[i] = SequenceList;
        }

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
            if (newPixel.X() >= 0 && newPixel.X() <= bitmap.Width && newPixel.Y() >= 0 && newPixel.Y() <= bitmap.Height)
                if (bitmap.GetPixel(newPixel.X(), newPixel.Y()) == Color.FromArgb(255, 0, 0))
                {
                    FindOutlineSequence(newPixel, bitmap, sequence);
                }
        }

    }

    internal static List<List<Coordinate>>[] GenerateFillingSequences(Bitmap[,] patternMaps)
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
            //if any of them is red
            if (pixel.X() - 1 >= 0 && pixel.X() + 1 <= bitmap.Width)
                if (bitmap.GetPixel(newPixel.X(), newPixel.Y()) == Color.FromArgb(0, 255, 0))
                {
                    FindOutlineSequence(newPixel, bitmap, sequence);
                }
        }
    }

}
