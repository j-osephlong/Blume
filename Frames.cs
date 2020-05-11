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
        public Frame (Grid G)
        {
            this.G = (Grid)G.Clone();
            this.Product = (Grid)G.Clone();
            
            this.Product = FrameTools.Flatten(this.Product);
            this.Product.ApplyLuminance();
        }
    }

    static class FrameTools
    {
        public static Grid Flatten (Grid G)
        {
            /*
                Create a flattened, top down, 2d grid from the 3d grid passed to the frame
                */

            Grid product = new Grid(G.width, G.height, 1);
            for (int z = G.depth - 1; z >= 0; z--)
                for (int y = 0; y < G.height; y++)
                    for (int x = 0; x < G.width; x++)
                    {
                        if (product[0, y, x] == null && G[z, y, x] != null)
                            product[0, y, x] = G[z, y, x];
                    }

            return product;
        }

        public static List<Coord> Contrast (Frame F1, Frame F2)
        {
            /*
                Find all differences between two frames.
                */

            //only contrasts flattned Frames
            List<Coord> changes = new List<Coord> ();

            for (int y = 0; y < F1.Product.height; y++)
                for (int x = 0; x < F1.Product.width; x++)
                    if (F1.Product[0, y, x].Character != F2.Product[0, y, x].Character
                        || 
                        F1.Product[0, y, x].ColorValue != F2.Product[0, y, x].ColorValue)
                        changes.Add(new Coord(x, y));

            return changes;
        }

        public static void ApplyLuminance (this Grid G)
        {
            double [,] LumMap = Luminate.GenLumMap(G);

            // for (int i = 0; i < 10; i++)
            //     Console.Write(LumMap[i, i]+" ");

            // Console.Read/sz();

            for (int y = 0; y < G.height; y++)
            {
                for (int x = 0; x < G.width; x++)
                {
                    Color C = G[0, y, x].ColorValue ?? default(Color);
                    Color newC = Color.FromArgb(
                            (int)(C.R * (LumMap[y, x]>Luminate.minLuminance ? LumMap[y, x] : Luminate.minLuminance)),
                            (int)(C.G * (LumMap[y, x]>Luminate.minLuminance ? LumMap[y, x] : Luminate.minLuminance)),
                            (int)(C.B * (LumMap[y, x]>Luminate.minLuminance ? LumMap[y, x] : Luminate.minLuminance))
                        );
                    
                    G[0, y, x].ColorValue = newC;
                }
            }
        }


    }

    class Luminate
    {
        public const double minLuminance = 0.15; 
        public const double decayFactor = 0.01; 

        public static List<Coord> Line (Coord CStart, Coord CEnd)
        {
            double dX = CEnd.x - CStart.x;
            double dY = CEnd.y - CStart.y;
            double dErr;
            double err = 0.0;

            List<Coord> L = new List<Coord>();
            Coord C1 = CStart;
            Coord C2 = CEnd;

            if (dY < 0)
                C2.y+= 2*((int)Math.Abs(dY));
            if (dX < 0)
                C2.x+= 2*((int)Math.Abs(dX));

            if ((C2.x-C1.x) > (C2.y-C1.y))
            {
                dErr = Math.Abs((double)(C2.y-C1.y)/(C2.x-C1.x));
                int y = C1.y;

                for (int x = C1.x; x < C2.x; x++)
                {
                    L.Add(new Coord(x, y));

                    err += dErr;
                    if (err >= 0.5)
                    {
                        y += 1;
                        err -= 1.0;
                    }
                }
            }
            else
            {
                dErr = Math.Abs((double)(C2.x-C1.x)/(C2.y-C1.y));
                int x = C1.x;

                for (int y = C1.y; y < C2.y; y++)
                {
                    L.Add(new Coord(x, y));

                    err += dErr;
                    if (err >= 0.5)
                    {
                        x += 1;
                        err -= 1.0;
                    }
                }
            }

            L.Add(C2);

            if (dY < 0)
                for (int i = 0; i < L.Count; i++)
                {
                    Coord C = L[i];
                    C.y-= 2*Math.Abs(C.y-C1.y);
                    L[i] = C;
                }

            if (dX < 0)
                for (int i = 0; i < L.Count; i++)
                {
                    Coord C = L[i];
                    C.x-= 2*Math.Abs(C.x-C1.x);
                    L[i] = C;
                }

            return L;
        }

        public static int GetMaxLumDist(double lum)
        {
            int maxLumDist = 0;
            double c = lum;

            while (Math.Round(c, 2) >= minLuminance)
            {
                maxLumDist++;
                c = Math.Pow(lum*(1-decayFactor), maxLumDist);
            }

            return maxLumDist;
        }

        public static double[,] GenLumMap (Grid G)
        {
            double [,] LumMap = new double [G.height, G.width];
            List<Coord> LuminantUnits = new List<Coord> ();

            for (int y = 0; y < G.height; y++)
                for (int x = 0; x < G.width; x++)
                    if (G[0, y, x].HasFlag("lum"))
                        LuminantUnits.Add(new Coord(x, y));

            foreach (Coord C in LuminantUnits)
            {
                double Luminance = G[C].GetFlag<double>("lum");

                int maxDist = GetMaxLumDist(Luminance);
    
                int xMinDist = C.x - maxDist >= 0? C.x - maxDist : 0;
                int xMaxDist = C.x + maxDist <= G.width? C.x + maxDist : G.width;
                int yMinDist = C.y - maxDist >= 0? C.y - maxDist : 0;
                int yMaxDist = C.y + maxDist <= G.height? C.y + maxDist : G.height;

                for (int y = yMinDist; y < yMaxDist; y++)
                {
                    for (int x = xMinDist; x < xMaxDist; x++)
                    {
                        List<Coord> ray = Line(C, new Coord(x, y));
                        int i = ray.Count;
                        foreach (Coord C2 in ray)
                        {
                            double l = Math.Pow(Luminance*(1-decayFactor), Math.Abs(C.x-C2.x)+Math.Abs(C.y-C2.y));
                            if (l > LumMap[C2.y, C2.x])
                            {
                                if (l < minLuminance)
                                    LumMap[C2.y, C2.x] = minLuminance;
                                else
                                    LumMap[C2.y, C2.x] = l;
                            }
                            if (G[C2].HasFlag("solid"))
                                break;
                            i--;
                        }
                    }
                }
            }

            return LumMap;
        }
    }

}