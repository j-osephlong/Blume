using System;
using System.Windows.Input;
using System.Drawing;
using Map;
using Frames;

class Blume 
{   
    public static void Main (string [] args)
    {
        Grid G = new Grid(20, 20, 2, new Unit ('\u2593', null));

        // double lumConstant = 0.75;

        // for (int y = 0; y < G.height; y++)
        //     for (int x = 0; x < G.width; x++)
        //     {
        //         int centerPos = G.width/2;
        //         double lumValue = Math.Pow(lumConstant, Math.Abs(y - centerPos) + Math.Abs(x - centerPos));
        //         G.posGrid[0, y, x].ColorValue = 
        //             Color.FromArgb((int)(255 * lumValue), (int)(255 * lumValue), (int)(255 * lumValue));
        //     } 
        // Renderer.Render(new Frame(G));       
         
        Renderer.Render(FrameTools.ReadImageBlocks("test.jpg", Console.WindowWidth, Console.WindowHeight));

    }
}