using System;
using System.Windows.Input;
using System.Drawing;
using Map;
using Frames;

class Blume 
{   

    public static void PlayGround ()
    {

        Console.WriteLine("End of play time.");
        Console.Read();
        Console.Read();
    }

    public static void Main (string [] args)
    {
        PlayGround();

        Grid G = new Grid(50, 30, 2, new Unit ('\u2593', Color.FromArgb(124, 44, 56)));
        Grid.SetLayer(G, 0, 0, FrameTools.ReadImageBlocks("test.jpg", 50, 50));

        // for (int y = 0; y < G.height; y++)
        // {
        //     for (int x = 0; x < G.width; x++)
        //     {
        //         Frame F = new Frame(G);
        //         var Path = FrameTools.Line(F.Product, x, y, G.width/2, G.height/2);
        //         foreach (var S in Path)
        //         {
        //             F.Product.posGrid[0, S.Item2, S.Item1].Character = 'X';
        //         }
        //         Renderer.Render(F);
        //         Console.Read();
        //     }
        // }
        // G.posGrid[0, 10, 10].Flags.Add("luminant");
        G.posGrid[0, 20, 30].Flags.Add("luminant");

        
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