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

}
