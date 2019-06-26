using System;
using System.Drawing;
using Map;
using Frames;
using Pastel;

class Renderer
{
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
        Console.Clear();

        for (int y = 0; y < F.Product.height; y++)
        {
            for (int u = 0; u < F.Product.width; u++)
            {
                if (F.Product.posGrid[0, y, u].ColorValue == null)
                    Console.Write(F.Product.posGrid[0, y, u].Character );
                else
                    Console.Write($"{(F.Product.posGrid[0, y, u].Character + "").Pastel(F.Product.posGrid[0, y, u].ColorValue ?? default(Color))}");
            }
            Console.WriteLine();
        }
    }
    
    public static void Update(Frame F)
    {
        foreach (var change in FrameTools.Contrast(PrevFrame, F))
        {
            // Console.WriteLine(change.Item1 + "-" + change.Item2);
            Console.SetCursorPosition(change.Item1, change.Item2);

            if (F.Product.posGrid[0, change.Item2, change.Item1].ColorValue == null)
                Console.Write(F.Product.posGrid[0, change.Item2, change.Item1].Character);
            else
                Console.Write($"{(F.Product.posGrid[0, change.Item2, change.Item1].Character + "").Pastel(F.Product.posGrid[0, change.Item2, change.Item1].ColorValue ?? default(Color))}");
            
        }
    }
}