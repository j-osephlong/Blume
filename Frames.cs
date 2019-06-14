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
            this.Product = FrameTools.Luminate(this.Product);
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

        public static Grid ReadImageBlocks (string path, int w, int h)
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
                    // product.posGrid[0, bY, bX].Flags.Add("luminant");
                    // Console.Write($"{"\u2593\u2593".Pastel(Color.FromArgb(AvgR, AvgG, AvgB))}");
                }
                // Console.WriteLine();
            }
            return product;
        }

        public static List<Tuple<int, int>> Line (Grid G, int x1, int y1, int x2, int y2)
        {
            List<Tuple<int, int>> Path = new List<Tuple<int, int>> ();
            int DeltaX = Math.Abs(x1-x2);
            int DeltaY = Math.Abs(y1-y2);

            double ROC = DeltaX < DeltaY ? (double)DeltaY/DeltaX : (double)DeltaX/DeltaY;
            char Dir = DeltaX < DeltaY ? 'Y' : 'X';

            int XDir = x1 < x2 ? 1 : (x1 == x2 ? 0 : -1);
            int YDir = y1 < y2 ? 1 : (y1 == y2 ? 0 : -1);

            int curX = x1; int curY = y1;
            
            double overflow = 0;
            while (curX != x2 && curY != y2)
            {
                if (Dir == 'Y')
                {
                    overflow += (ROC - (int)ROC);
                    for (int i = 1; i <= ((int)ROC + (int)overflow); i++)
                    {
                        Path.Add(Tuple.Create(curX, curY));
                        // G.posGrid[0, curY, curX].Character = 'O';
                        curY+=YDir;
                    }
                    if (overflow >= 1)
                        overflow = overflow - (int)overflow;
                    curX+=XDir;
                    // G.posGrid[0, curY, curX].Character = 'O';
                }

                if (Dir == 'X')
                {
                    overflow += (ROC - (int)ROC);
                    for (int i = 1; i <= ((int)ROC + (int)overflow); i++)
                    {
                        Path.Add(Tuple.Create(curX, curY));
                        // G.posGrid[0, curY, curX].Character = 'O';
                        curX+=XDir;
                    }
                    if (overflow >= 1)
                        overflow = overflow - (int)overflow;
                    curY+=YDir;
                    // G.posGrid[0, curY, curX].Character = 'O';
                }
            }

            // if (x1 == x2)
            // {
            //     Path.Clear();
            //     for (int y = y1; y <= y2; y++)
            //         Path.Add(Tuple.Create(x1, y));
            //     return Path;                
            // } 
            // else if (y1 == y2)
            // {
            //     Path.Clear();
            //     for (int x = x1; x <= x2; x++)
            //         Path.Add(Tuple.Create(x, y1));
            //     return Path;
            // }
            // else
                Path.Add(Tuple.Create(curX, curY));
            // G.posGrid[0, curY, curX].Character = 'O';   
            Path.Reverse();
            return Path;
        }

        public static Grid Luminate (Grid G)
        {
            double [,] lumMap = new double [G.height, G.width];
            double lumConstant = 0.90;        

            for (int y = 0; y < G.height; y++)
                for (int x = 0; x < G.width; x++)
                {
                    if (G.posGrid[0, y, x].Flags.Contains("luminant"))
                    {
                        // Console.WriteLine(y + " " + x);
                        // Console.Read();
                        for (int y2 = 0; y2 < G.height; y2++)
                            for (int x2 = 0; x2 < G.width; x2++)
                            {
                                var Path = Line(G, x2, y2, x, y);
                                int distance = Math.Abs(x2 - x) + Math.Abs(y2 - y);

                                bool reachable = true;
                                for (int i = 0; i < Path.Count - 1; i++)
                                {
                                    if (G.posGrid[0, Path[i].Item2, Path[i].Item1].Flags.Contains("wall"))
                                    {
                                        // Console.WriteLine(S.Item1 + " " + S.Item2 + " w");
                                        reachable = false;
                                    }
                                }

                                if (!reachable)
                                {
                                    // lumMap[y2, x2] = 0;
                                }
                                else if (lumMap[y2, x2] < Math.Pow(lumConstant, distance))
                                    lumMap[y2, x2] = Math.Pow(lumConstant, distance);
                            }
                    }
                }

            for (int y = 0; y < G.height; y++)
            {
                for (int x = 0; x < G.width; x++)
                {
                    Color oldColor = G.posGrid[0, y, x].ColorValue ?? default(Color);
                    Color newColor = Color.FromArgb(
                            (int)(oldColor.R * lumMap[y, x]),
                            (int)(oldColor.G * lumMap[y, x]),
                            (int)(oldColor.B * lumMap[y, x])
                        );
                    G.posGrid[0, y, x].ColorValue = newColor;
                }
            }

            // for (int y = 0; y < G.height; y++)
            // {
            //     for (int x = 0; x < G.width; x++)
            //     {
            //         Console.Write($"{('\u2593'+"").Pastel(G.posGrid[0, y, x].ColorValue ?? default(Color))}");
            //     }
            //     Console.WriteLine();
            // }
            // Console.Read();

            return G;
        }
    }
}