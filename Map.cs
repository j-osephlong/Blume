using System;
using System.Drawing;
using System.Collections.Generic; 
using System.IO;
using System.Text;
// using Pastel;

namespace Map
{
    [Serializable]
    class Unit : ICloneable
    {
        /*
            A unit is a single point on a grid, with it's own Flag, Sprite, character, and color 
            information.
            */

        public List<string> Flags;
        public char Character {get; set;}
        public Color? ColorValue {get; set;}
        public Grid Sprite {get; private set;}
        public bool HasSprite {get; private set;}
        public string currentSprite {get; private set;}
        public Dictionary<string, Grid> Sprites;
        public double lumConstant {get; set;}

        public Unit (char Character, Color? ColorValue = null)
        {
            this.Character = Character;
            this.ColorValue = ColorValue;
            this.Flags = new List<string>();
            this.Sprites = new Dictionary<string, Grid> ();

            this.HasSprite = false;
            this.Sprite = null;
            this.currentSprite = null;

            lumConstant = 0.95;
        }

        public void AddSprite (string name, Grid S)
        {
            Sprites.Add(name, (Grid)S.Clone());
        }

        public void SetSprite (string name)
        {
            Grid S;
            HasSprite = true;
            if (Sprites.TryGetValue(name, out S))
            {
                Sprite = S;
                currentSprite = name;
            }
            else
            {
                HasSprite = false;
                currentSprite = null;
            }
        }

        public object Clone ()
        {
            Unit C = (Unit)this.MemberwiseClone();
            C.Flags = new List<string>();
            foreach(string flag in Flags)
                C.Flags.Add(flag);
            return (object)C;
        }

        public void ReadOut()
        {
            Console.Clear();
            string str = "";
            str = "Character: (int)" + (int)this.Character + " (char)" + this.Character;
            Console.WriteLine(str);
            str = "Color: " + this.ColorValue.ToString();
            Console.WriteLine(str);
            if (Flags.Count != 0)
            {    
                str = "Flags:";
                Console.WriteLine(str);
                foreach (string flag in this.Flags)
                {
                    str = "\t" + flag;
                    Console.WriteLine(str);
                }
            }
            str = "LumConstant: " + this.lumConstant;
            Console.WriteLine(str);
            str = "Has Sprite: " + this.HasSprite;
            Console.WriteLine(str);
        }

    }

    [Serializable]
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

        public static Grid LoadGrid (string path)
        {
            using (StreamReader sr = File.OpenText(path))
            {
                int depth, height, width;
                depth = System.Convert.ToInt32(sr.ReadLine());
                height = System.Convert.ToInt32(sr.ReadLine());
                width = System.Convert.ToInt32(sr.ReadLine());

                Grid G = new Grid(width, height, depth, new Unit('X'));

                return G;
            }
        }

        public void SaveGrid (string path)
        {
            if (File.Exists(path))
                File.Delete(path);

            // Create the file.
            using (FileStream fs = File.Create(path))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(this.depth+"\n"+this.height+"\n"+this.width+"\n");
                fs.Write(info, 0, info.Length);
                int count = 0;
                foreach (Unit U in this.posGrid)
                {
                    if (count % (this.height * this.width) == 0)
                    {
                        info = new UTF8Encoding(true).GetBytes("::z_break::\n");
                        fs.Write(info, 0, info.Length);                                            
                    }
                    else if (count % this.width == 0)
                    {
                        info = new UTF8Encoding(true).GetBytes("::y_break::\n");
                        fs.Write(info, 0, info.Length); 
                    }

                    if (U == null)
                        info = new UTF8Encoding(true).GetBytes("null\n");
                    else
                    {
                        info = new UTF8Encoding(true).GetBytes((int)U.Character+"/"+U.ColorValue.ToString()+"/"+U.lumConstant+"\n");                        
                    }
                    fs.Write(info, 0, info.Length);
                    count++;                                      
                }
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
                        if (this.posGrid[z, y, u] != null)
                            posGridClone[z, y, u] = (Unit)clone.posGrid[z, y, u].Clone();
                        else 
                            posGridClone[z, y, u] = null;
            clone.posGrid = posGridClone;

            return (object)clone;
        }

        public static void SetLayer (Grid G, int layer, int xOffset, int yOffset, Grid Slide)
        {
            for (int y = 0; y < (G.height <= Slide.height ? G.height : Slide.height) - yOffset; y++)
            {
                for (int x = 0; x < (G.width <= Slide.width ? G.width : Slide.width) - xOffset; x++)
                {
                    try
                    {
                        G.posGrid[layer, y + yOffset, x + xOffset] = Slide.posGrid[0, y, x];
                    }
                    catch (Exception e)
                    {

                    }
                }
            }
        }
    }
}