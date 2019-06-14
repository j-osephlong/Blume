using System;
using System.Drawing;
using System.Collections.Generic; 
// using Pastel;

namespace Map
{
    class Unit : ICloneable
    {
        public List<string> Flags;
        public char Character {get; set;}
        public Color? ColorValue {get; set;}

        public Unit (char Character, Color? ColorValue = null)
        {
            this.Character = Character;
            this.ColorValue = ColorValue;
            this.Flags = new List<string>();
        }

        public object Clone ()
        {
            Unit C = (Unit)this.MemberwiseClone();
            C.Flags = new List<string>();
            foreach(string flag in Flags)
                C.Flags.Add(flag);
            return (object)C;
        }
    }

    class Grid : ICloneable
    {
        public Unit [,,] posGrid;
        public int width, height, depth;

        public Grid (int width, int height, int depth, Unit defaultUnit)
        {
            this.width = width;
            this.height = height;
            this.depth = depth;
            
            posGrid = new Unit [depth, height, width];
            for (int y = 0; y < height; y++)
                for (int u = 0; u < width; u++)
                    if (defaultUnit == null)
                        posGrid[0, y, u] = null;
                    else 
                        posGrid[0, y, u] = (Unit)defaultUnit.Clone();

            if (depth > 1)
                for (int z = 1; z < depth; z++)
                    for (int y = 0; y < height; y++)
                        for (int u = 0; u < width; u++)
                            posGrid[z, y ,u] = null;
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
                        if (this.posGrid[z, y, u] != null)
                            posGridClone[z, y, u] = (Unit)clone.posGrid[z, y, u].Clone();
                        else 
                            posGridClone[z, y, u] = null;
            clone.posGrid = posGridClone;

            return (object)clone;
        }

        public static void SetLayer (Grid G, int layer, int xOffset, Grid Slide)
        {
            for (int y = 0; y < (G.height <= Slide.height ? G.height : Slide.height); y++)
            {
                for (int x = 0; x < (G.width <= Slide.width ? G.width : Slide.width) - xOffset; x++)
                {
                    G.posGrid[layer, y, x + xOffset] = Slide.posGrid[layer, y, x];
                }
            }
        }
    }
}