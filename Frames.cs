using System;
using System.Drawing;
using System.Collections.Generic;
using Pastel; 
using Map;

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace Frames
{
    class Frame
    {
        /*
            A frame is wrapper for a Grid, where further graphical processing may be applied, before 
            the frame is passed into the Renderer
            */

        public Grid G;
        public Grid Product;

        //final step must be to flatten Grid
        public Frame (Grid G, bool Lum = true)
        {
            this.G = (Grid)G.Clone();
            this.Product = (Grid)G.Clone();
            this.Product = FrameTools.ApplySprites(this.Product);
            // this.Product = FrameTools.Sparkle(this.Product, 5, 0);
            this.Product = FrameTools.Flatten(this.Product);
            if (Lum)
                this.Product = FrameTools.Luminate(this.Product);
        }
    }

    static class FrameTools
    {
        public static Grid Flatten (Grid G)
        {
            /*
                Create a flattened, top down, 2d grid from the 3d grid passed to the frame
                */

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

        public static Grid ApplySprites (Grid G)
        {
            /*
                Every unit in a grid may have a sprite attached to it, which is another grid.
                The sprite is not applied to the grid before it is passed to a frame for the
                purpose of preserving position data.
                */

            Grid tempG = (Grid)G.Clone();
            for (int z = 0; z < G.depth; z++)
                for (int y = 0; y < G.height; y++)
                {
                    for (int x = 0; x < G.width; x++)
                    {
                        if (G.posGrid[z, y, x] != null)
                            if (G.posGrid[z, y, x].HasSprite)
                            {
                                for (int sy = 0; sy < G.posGrid[z, y, x].Sprite.height; sy++)
                                {
                                    if (y + sy < G.height)
                                        for (int sx = 0; sx < G.posGrid[z, y, x].Sprite.width; sx++)
                                        {
                                            if (x + sx < G.width)
                                                if (G.posGrid[z, y, x].Sprite.posGrid[0, sy, sx] != null)
                                                    tempG.posGrid[z, y + sy, x + sx] = (Unit)G.posGrid[z, y, x].Sprite.posGrid[0, sy, sx].Clone();
                                                else 
                                                    tempG.posGrid[z, y + sy, x + sx] = null;
                                        }
                                }
                            }
                    }
                }

            return tempG;
        }

        public static List<Tuple<int, int>> Contrast (Frame F1, Frame F2)
        {
            /*
                Find all differences between two frames.
                */

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
            /*
                Create a grid based off of a provided image file.
                */
            string binPath = "ImageAproximations/"+path.Substring(path.LastIndexOf('/')+1, path.LastIndexOf('.')-path.LastIndexOf('/')-1)+".bin";
            Directory.CreateDirectory("ImageAproximations");
            if (File.Exists(binPath))
                return (Grid)new BinaryFormatter().Deserialize(new FileStream(binPath,FileMode.Open,FileAccess.Read));

            Grid product = new Grid(w, h, 1, new Unit('\u2593'));

            Bitmap image = new Bitmap(path);
            // image = Blur(image, -1);
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
                    int AvgA = 0;
                    int pixelCount = 0;
                    for (int y = (hR * bY); y < hR + (hR * bY); y++)
                        for (int x = (wR * bX); x < wR + (wR * bX); x++)
                        {
                            // Console.WriteLine((wR + (wR * bX)));
                            if (x % 2 == 0)
                            {
                                Color pixel = image.GetPixel(x ,y);
                                AvgR += pixel.R;
                                AvgG += pixel.G;
                                AvgB += pixel.B;
                                AvgA += pixel.A;
                                pixelCount++;
                            }
                        }

                    AvgR = AvgR/pixelCount;
                    AvgG = AvgG/pixelCount;
                    AvgB = AvgB/pixelCount;
                    AvgA = AvgA/pixelCount;
                    // Console.WriteLine(AvgR+"-"+AvgG+"-"+AvgB);
                    if (AvgA >225)
                        product.posGrid[0, bY, bX].ColorValue = Color.FromArgb(AvgR, AvgG, AvgB);
                    else    
                        product.posGrid[0, bY, bX] = null;
                    // product.posGrid[0, bY, bX].Flags.Add("luminant");
                    // Console.Write($"{"\u2593\u2593".Pastel(Color.FromArgb(AvgR, AvgG, AvgB))}");
                }
                // Console.WriteLine();
            }

            
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(binPath,FileMode.Create,FileAccess.Write);
            
            formatter.Serialize(stream, product);
            stream.Close();
            
            return product;
        }

        public static LinkedList<Tuple<int, int>> Line (Grid G, int x1, int y1, int x2, int y2)
        {
            /*
                Draw a line between two points and create a list of coordinates for said line.
                */

            LinkedList<Tuple<int, int>> Path = new LinkedList<Tuple<int, int>> ();
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
                        Path.AddLast(Tuple.Create(curX, curY));
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
                        Path.AddLast(Tuple.Create(curX, curY));
                        // G.posGrid[0, curY, curX].Character = 'O';
                        curX+=XDir;
                    }
                    if (overflow >= 1)
                        overflow = overflow - (int)overflow;
                    curY+=YDir;
                    // G.posGrid[0, curY, curX].Character = 'O';
                }
            }

            if (x1 == x2)
            {
                Path.Clear();
                for (int y = (y1 < y2 ? y1 : y2);  (y1 < y2 ? y <= y2 : y >= y1); y+= (y1 < y2 ? 1 : -1))
                    Path.AddLast(Tuple.Create(x1, y));
                return Path;                
            } 
            else if (y1 == y2)
            {
                Path.Clear();
                for (int x = (x1 < x2 ? x1 : x2); (x1 < x2 ? x <= x2 : x >= x1); x+= (x1 < x2 ? 1 : -1))
                    Path.AddLast(Tuple.Create(x, y1));
                return Path;
            }
            else
                Path.AddLast(Tuple.Create(curX, curY));
            // G.posGrid[0, curY, curX].Character = 'O';   
            return Path;
        }

        public static Grid Sparkle (Grid G, int maxPoints, int layer)
        {
            Random rnd = new Random();
            while (maxPoints > 0)
            {
                int x = rnd.Next(0, G.width);
                int y = rnd.Next(0, G.height);
                G.posGrid[layer, y, x].Flags.Add("luminant");
                G.posGrid[layer, y, x].lumConstant = 0.5;
                
                maxPoints--;
            }

            return G;
        }

        public static Grid Luminate (Grid G)
        {
            /*
                Apply lumination effect, inefficiently.
                */
            
                
            double [,] lumMap = new double [G.height, G.width];  
            double minLum = 0.20;   
            int maxLumDistance = 0;   

            for (int y = 0; y < G.height; y++)
                for (int x = 0; x < G.width; x++)
                {
                    if (G.posGrid[0, y, x].Flags.Contains("luminant"))
                    {
                        maxLumDistance = 0;
                        double c = G.posGrid[0, y, x].lumConstant;
                        while (Math.Round(c, 2) >= minLum)
                        {
                            maxLumDistance++;
                            c = Math.Pow(G.posGrid[0, y, x].lumConstant, maxLumDistance);
                        }
                        // Console.WriteLine(maxLumDistance);
                        // Console.ReadLine();

                        for (int y2 = (y - maxLumDistance) >= 0 ? (y - maxLumDistance) : 0; y2 < ((y + maxLumDistance) <= G.height ? (y + maxLumDistance) : G.height); y2++)
                            for (int x2 = (x - maxLumDistance) >= 0 ? (x - maxLumDistance) : 0; x2 < ((x + maxLumDistance) <= G.width ? (x + maxLumDistance) : G.width); x2++)
                            {
                                var Path = Line(G, x2, y2, x, y);
                                int distance = Math.Abs(x2 - x) + Math.Abs(y2 - y);

                                bool reachable = true;
                                for (int i = 0; i < Path.Count - 1; i++)
                                {
                                    var Coord = Path.First;
                                    Path.RemoveFirst();
                                    if (G.posGrid[0, Coord.Value.Item2, Coord.Value.Item1].Flags.Contains("wall"))
                                    {
                                        reachable = false;
                                    }
                                }

                                if (!reachable)
                                {
                                    // lumMap[y2, x2] = 0;
                                }
                                else if (lumMap[y2, x2] < Math.Pow(G.posGrid[0, y, x].lumConstant, distance))
                                    lumMap[y2, x2] = Math.Pow(G.posGrid[0, y, x].lumConstant, distance);
                            }
                    }
                }

            for (int y = 0; y < G.height; y++)
            {
                for (int x = 0; x < G.width; x++)
                {
                    double l = lumMap[y, x];
                    if (l < minLum)
                        l = minLum;
                    Color oldColor = G.posGrid[0, y, x].ColorValue ?? default(Color);
                    Color newColor = Color.FromArgb(
                            (int)(oldColor.R * l),
                            (int)(oldColor.G * l),
                            (int)(oldColor.B * l)
                        );
                    G.posGrid[0, y, x].ColorValue = newColor;
                }
            }

            return G;
        }
    }
}