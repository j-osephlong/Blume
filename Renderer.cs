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
                if (F.Product.posGrid[0, y, u].ColorValue == null)
                    Console.Write(F.Product.posGrid[0, y + yOffset, u + xOffset].Character );
                else
                    Console.Write($"{(F.Product.posGrid[0, y + yOffset, u + xOffset].Character + "").Pastel(F.Product.posGrid[0, y + yOffset, u + xOffset].ColorValue ?? default(Color))}");
            }
            Console.WriteLine();
        }
    }
    
    public static void Update(Frame F)
    {
        int prevX = 0;
        int prevY = 0;
        bool firstChange = true;
        foreach (var change in FrameTools.Contrast(PrevFrame, F))
        {
            if (firstChange)
                Console.SetCursorPosition(change.Item1, change.Item2);

            if (change.Item2 != prevY || change.Item1 != prevX + 1)
                Console.SetCursorPosition(change.Item1, change.Item2);
            
            if (F.Product.posGrid[0, change.Item2, change.Item1].ColorValue == null)
                Console.Write(F.Product.posGrid[0, change.Item2, change.Item1].Character);
            else
                Console.Write($"{(F.Product.posGrid[0, change.Item2, change.Item1].Character + "").Pastel(F.Product.posGrid[0, change.Item2, change.Item1].ColorValue ?? default(Color))}");
            
            prevX = change.Item1;
            prevY = change.Item2;
            firstChange = false;
        }
    }
}