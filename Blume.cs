using System;
using System.Windows.Input;
using System.Drawing;
using Map;
using Frames;

class Blume 
{   
    public static void Main (string [] args)
    {
        Grid G = new Grid(50, 50, 2, new Unit ('\u2593', Color.FromArgb(124, 44, 56)));
        G.posGrid[0, 10, 10].Flags.Add("luminant");
        G.posGrid[0, 20, 30].Flags.Add("luminant");
        G.posGrid[0, 5, 5].Flags.Add("wall");
        G.posGrid[0, 5, 5].Character = 'X';
        G.posGrid[0, 6, 5].Character = 'X';
        G.posGrid[0, 7, 5].Character = 'X';
        G.posGrid[0, 6, 5].Flags.Add("wall");
        G.posGrid[0, 7, 5].Flags.Add("wall");
        // FrameTools.Luminate(G);
        // Console.Read();
        // Console.Read();

        // for (int y = 0; y < G.height; y++)
        // {
        //     for (int x = 0; x < G.width; x++)
        //     {
        //         Frame F = new Frame(G);
        //         FrameTools.Line(F.Product, x, y, G.width/2, G.height/2);
        //         Renderer.Render(F);
        //         Console.Read();
        //     }
        // }
        Player P = new Player(G, new Unit('X', Color.Aqua));

        P.Place(10, 10, 1);

        KeyboardInput.Toggleables.Add(ConsoleKey.E);

        while (true)
        {
            KeyboardInput.Read();
            P.PlayBall();
            Renderer.Render(new Frame(G));
        }

        
        // Renderer.Render(new Frame(G));       
         
        // Renderer.Render(FrameTools.ReadImageBlocks("test.jpg", Console.WindowWidth, Console.WindowHeight));

    }
}