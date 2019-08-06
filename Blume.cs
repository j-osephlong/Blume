using System;
using System.Windows.Input;
using System.Drawing;
using Map;
using Frames;

class Blume 
{   
    public static int WIDTH = 220/2;
    public static int HEIGHT = 80/2;

    public static void PlayGround ()
    {
        //Test, debug code, kept separate from the main function for readability purposes
        while (true)
        {
            Grid G = new Grid(WIDTH, HEIGHT, 4, new Unit ('\u2593', Color.FromArgb(124, 44, 56)));
            Grid.SetLayer(G, 0, 0, 0, FrameTools.ReadImageBlocks(Console.ReadLine(), G.width, G.height));
            Grid.SetLayer(G, 1, 0, 0, FrameTools.ReadImageBlocks(Console.ReadLine(), G.width, G.height));
            Renderer.Render(new Frame (G, Lum:false));
        }
        // Grid G = new Grid(WIDTH, HEIGHT, 4, new Unit ('\u2593', Color.FromArgb(124, 44, 56)));
        // G.SaveGrid("h.txt");
        Console.Read();
        Console.Read();
    }

    public static void Main (string [] args)
    {
        start:
        // PlayGround();

        Grid G = new Grid(WIDTH, HEIGHT, 4, new Unit ('\u2593', Color.FromArgb(124, 44, 56)));
        
        Player P = new Player(G, new Unit('X', Color.Aqua));

        P.Place(10, 1, 1);
        // G.posGrid[1,1, 10].Sprite.SaveGrid("h.txt");
        KeyboardInput.Toggleables.Add(ConsoleKey.E);
        Clock.QuarterSecondElapsed += P.MoveBG;
        Clock.Start();
        while (true)
        {
            KeyboardInput.Read();
            // MouseInput.Read();
            P.PlayBall();
            Clock.Tick();
            Renderer.Render(new Frame(G, Lum:false));

            if (KeyboardInput.IsPressed(ConsoleKey.R, ConsoleModifiers.Alt))
                goto start;
        }
    }
}