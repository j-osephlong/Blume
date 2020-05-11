using System;
using System.Drawing;
using System.Collections.Generic; 
using System.IO;
using System.Text;
// using Pastel;

namespace Map
{
    public struct Coord
    {
        public int x;
        public int y;
        public int? z;

        public Coord (int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Coord (int x, int y)
        {
            this.x = x;
            this.y = y;
            this.z = null;
        }
    }

    [Serializable]
    public class Unit : ICloneable
    {
        /*
            A unit is a single point on a grid, with it's own Flag, Sprite, character, and color 
            information.
            */

        public Dictionary<string, object> Flags;
        public char Character {get; set;}
        public Color? ColorValue {get; set;}

        public Unit (char Character, Color? ColorValue = null)
        {
            this.Character = Character;
            this.ColorValue = ColorValue;
            this.Flags = new Dictionary<string, object>();
        }

        public T GetFlag<T>(string flag)
        {
            return (T)Flags[flag];
        }

        public void SetFlag (string flag, object value)
        {
            if (Flags.ContainsKey(flag))
                Flags.Remove(flag);
            Flags.Add(flag, value);
        }

        public bool HasFlag(string flag)
        {
            return Flags.ContainsKey(flag);
        }

        public object Clone ()
        {
            Unit C = (Unit)this.MemberwiseClone();
            C.Flags = new Dictionary<string, object>();
            foreach(string flag in Flags.Keys)
                C.Flags.Add(flag, Flags[flag]);
            return (object)C;
        }
    }

    [Serializable]
    public class Grid : ICloneable
    {
        public Unit [,,] posGrid;
        public int width, height, depth;

        public Grid (int width, int height, int depth)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;

            this.posGrid = new Unit [this.depth, this.height, this.width];
            
            for (int z = 0; z < this.depth; z++)
                for (int y = 0; y < this.height; y++)
                    for (int x = 0; x < this.width; x++)
                        this[z, y, x] = null;
        }

        public Unit this[int z, int y, int x]
        {
            get
            {
                return posGrid[z, y, x];
            }

            set
            {
                posGrid[z, y, x] = value;
            }
        }

        public Unit this[Coord C]
        {
            get
            {
                return posGrid[C.z ?? 0, C.y, C.x];
            }

            set
            {
                posGrid[C.z ?? 0, C.y, C.x] = value;
            }
        }

        public object Clone()
        {
            Grid clone = (Grid)this.MemberwiseClone();
            
            /*
                memberwiseclone doesn't clone arrays, so we must clone each 
                individual element (unit) to a new array.
                 */
            Unit [,,] posGridClone = new Unit [this.depth, this.height, this.width];
            for (int z = 0; z < depth; z++)
                for (int y = 0; y < height; y++)
                    for (int u = 0; u < width; u++)
                        if (this[z, y, u] != null)
                            posGridClone[z, y, u] = (Unit)clone[z, y, u].Clone();
                        else 
                            posGridClone[z, y, u] = null;
            clone.posGrid = posGridClone;

            return (object)clone;
        }
    }

    public static class GridTools
    {
        public static void FillLayer (this Grid G, int z, Unit u)
        {
            for (int y = 0; y < G.height; y++)
                for (int x = 0; x < G.width; x++)
                {
                    G[z, y, x] = (Unit)u.Clone();
                }
        }

        public static void SetLayer (this Grid G, int z, Grid B)
        {
            for (int y = 0; y < G.height; y++)
                for (int x = 0; x < G.width; x++)
                {
                    G[z, y, x] = (Unit)B[z, y, x].Clone();
                }
        }

        public static Grid ApproximateImage(string path, int width, int height)
        {
            Grid product = new Grid(width, height, 1);
            product.FillLayer(0, new Unit('\u2593'));

            Bitmap image = new Bitmap(path);

            int wR = image.Width / width;
            int hR = image.Height / height;

            for (int bY = 0; bY < height; bY++)
            {
                for (int bX = 0; bX < width; bX++)
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
                    
                    if (AvgA >225)
                        product[0, bY, bX].ColorValue = Color.FromArgb(AvgR, AvgG, AvgB);
                    else    
                        product[0, bY, bX].ColorValue = null;
                }
            }

            return product;
        }
    }
}