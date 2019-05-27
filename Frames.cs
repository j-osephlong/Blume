using System;
using System.Drawing;
using System.Collections.Generic;
using Pastel; 
using Map;

namespace Frames
{
    class Frame
    {
        public Grid G;
        public Grid Product;

        //final step must be to flatten Grid
        public Frame (Grid G)
        {
            this.G = (Grid)G.Clone();
            this.Product = (Grid)G.Clone();
            this.Product = FrameTools.Flatten(this.Product);
        }
    }

    static class FrameTools
    {
        public static Grid Flatten (Grid G)
        {
            Grid product = new Grid(G.width, G.height, 1, null);
            for (int z = G.depth - 1; z >= 0; z--)
                for (int y = 0; y < G.height; y++)
                    for (int x = 0; x < G.width; x++)
                    {
                        if (product.posGrid[0, y, x] == null && G.posGrid[z, y, x] != null)
                            product.posGrid[0, y, x] = G.posGrid[z, y, x];
                    }

            return product;
        }

        public static void line (this Grid G, int x1, int y1, int x2, int y2)
        {
            const int Layer = 0;
            int DX = Math.Abs(x1 - x2);
            int DY = Math.Abs(y1 - y2);
            char Direction;
            double rot = (double)DY/DX;
            int xDir;
            int yDir;

            if (rot < 1)
            {
                Direction = 'X';
                rot = 1/rot;
                if (double.IsInfinity(rot))
                    rot = DX;
            } 
            else 
            {
                Direction = 'Y';
                if (double.IsInfinity(rot))
                    rot = DY;
            }
            
            xDir = x1 > x2 ? 1 : -1;
            yDir = y1 > y2 ? 1 : -1;

            for (int i = 0; i < 
                (Direction == 'Y' ? Math.Ceiling(DY / rot) : Math.Ceiling(DX / rot));
                i++)
            {
                Console.WriteLine("HI"+(y1 + yDir*i));
            }
        }

        public static List<Tuple<int, int>> Contrast (Frame F1, Frame F2)
        {
            //only contrasts flattned Frames
            List<Tuple<int, int>> changes = new List<Tuple<int, int>> ();

            for (int y = 0; y < F1.Product.height; y++)
                for (int x = 0; x < F1.Product.width; x++)
                    if (F1.Product.posGrid[0, y, x].Character != F2.Product.posGrid[0, y, x].Character
                        || F1.Product.posGrid[0, y, x].ColorValue != F2.Product.posGrid[0, y, x].ColorValue)
                        changes.Add(Tuple.Create(x, y));

            return changes;
        }

        public static Frame ReadImageBlocks (string path, int w, int h)
        {
            Grid product = new Grid(w, h, 1, new Unit('\u2593'));

            Bitmap image = new Bitmap(path);
            int wR = image.Width / w;
            int hR = image.Height / h;
            // Console.WriteLine("wR - " + wR + " hR - " + hR + "\nimgW - " + image.Width + " imgH - " + image.Height);
            for (int bY = 0; bY < h; bY++)
            {
                for (int bX = 0; bX < w; bX++)
                {
                    int AvgR = 0;
                    int AvgG = 0;
                    int AvgB = 0;
                    int pixelCount = 0;
                    for (int y = (hR * bY); y < hR + (hR * bY); y++)
                        for (int x = (wR * bX); x < wR + (wR * bX); x++)
                        {
                            // Console.WriteLine((wR + (wR * bX)));
                        
                            Color pixel = image.GetPixel(x ,y);
                            AvgR += pixel.R;
                            AvgG += pixel.G;
                            AvgB += pixel.B;
                            pixelCount++;
                        }

                    AvgR = AvgR/pixelCount;
                    AvgG = AvgG/pixelCount;
                    AvgB = AvgB/pixelCount;
                    // Console.WriteLine(AvgR+"-"+AvgG+"-"+AvgB);
                    product.posGrid[0, bY, bX].ColorValue = Color.FromArgb(AvgR, AvgG, AvgB);
                    // Console.Write($"{"\u2593\u2593".Pastel(Color.FromArgb(AvgR, AvgG, AvgB))}");
                }
                // Console.WriteLine();
            }
            return new Frame(product);
        }

    }
}