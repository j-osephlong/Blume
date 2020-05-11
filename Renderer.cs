using System;
using System.Drawing;
using Map;
using Frames;
using Pastel;

class Renderer
{
    private static int Width = 80;
    private static int Height = 40;
    private static Frame PrevFrame = null;
    
    public static void Render (Frame F)
    {
        /*
            The renderer will only render flattened frames.
            */
            
        if (PrevFrame == null)
            PrintFrame(F);
        else 
            Update(F);

        PrevFrame = F;
    }

    public static void PrintFrame(Frame F)
    {
        int xOffset = 0;
        int yOffset = 0;

        Console.Clear();

        for (int y = 0; y < F.Product.height; y++) //F.Product.height
        {
            for (int u = 0; u < F.Product.width; u++) //F.Product.width
            {
                if (F.Product[0, y, u].ColorValue == null)
                    Console.Write(F.Product[0, y + yOffset, u + xOffset].Character );
                else
                    Console.Write($"{(F.Product[0, y + yOffset, u + xOffset].Character + "").Pastel(F.Product[0, y + yOffset, u + xOffset].ColorValue ?? default(Color))}");
            }
            Console.WriteLine();
        }
    }
    
    public static void Update(Frame F)
    {
        //change Product indicing to indice with coord struct
        foreach (Coord change in FrameTools.Contrast(PrevFrame, F))
        {
            Console.SetCursorPosition(change.x, change.y);
            
            if (F.Product[change].ColorValue == null)
                Console.Write(F.Product[change].Character);
            else
                Console.Write($"{(F.Product[change].Character + "").Pastel(F.Product[change].ColorValue ?? default(Color))}");

        }

        PrevFrame = F;
    }
}