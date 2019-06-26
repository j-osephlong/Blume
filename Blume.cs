using System;
using System.Windows.Input;
using System.Drawing;
using Map;
using Frames;

class Blume 
{   
    public static int WIDTH = 160/2;
    public static int HEIGHT = 80/2;

    public static void PlayGround ()
    {
        //Test, debug code, kept separate from the main function for readability purposes

        // Console.SetBufferSize(150, 150);
        Console.SetWindowSize(WIDTH, HEIGHT + 1);
        // Console.SetBufferSize(80, 40 + 1);

        Console.Read();
        Console.Read();
    }

    public static void Main (string [] args)
    {
        // PlayGround();

        Grid G = new Grid(WIDTH, HEIGHT, 4, new Unit ('\u2593', Color.FromArgb(124, 44, 56)));
        Grid.SetLayer(G, 0, 0, 0, FrameTools.ReadImageBlocks("1.jpg", G.width, G.height));
        Grid.SetLayer(G, 2, 0, 0, FrameTools.ReadImageBlocks("2.png", G.width, G.height));
        
        Player P = new Player(G, new Unit('X', Color.Aqua));

        P.Place(10, 10, 1);
        G.posGrid[1, 10, 10].SetSprite(new Grid(5, 5, 1, new Unit ('Q', Color.BlanchedAlmond)));

        KeyboardInput.Toggleables.Add(ConsoleKey.E);

        while (true)
        {
            KeyboardInput.Read();
            P.PlayBall();
            Renderer.Render(new Frame(G, Lum:false));
        }
    }
}