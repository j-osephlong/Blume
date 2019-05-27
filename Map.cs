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
            return this.MemberwiseClone();
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
    }
}