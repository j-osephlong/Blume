using System;
using System.Windows.Input;
using System.Drawing;
using Map;
using Frames;

class Blume 
{   
    public static void Main (string [] args)
    {
        Grid G = new Grid(32, 32, 2, new Unit ('\u2593', null));
        // FrameTools.Line(G, 19, 0, 19, 8);
        //     Renderer.Render(new Frame(G));
        //     Console.Read();
        for (int y = 0; y < G.height; y++)
        {
            for (int x = 0; x < G.width; x++)
            {
                Frame F = new Frame(G);
                FrameTools.Line(F.Product, x, y, G.width/2, G.height/2);
                Renderer.Render(F);
                Console.Read();
            }
        }
        Player P = new Player(G, new Unit('X'));

        P.Place(10, 10, 1);

        KeyboardInput.Toggleables.Add(ConsoleKey.E);

        double lumConstant = 0.65;        

        for (int y = 0; y < G.height; y++)
            for (int x = 0; x < G.width; x++)
            {
                int centerPos = G.width/2;
                double lumValue = Math.Pow(lumConstant, Math.Abs(y - centerPos) + Math.Abs(x - centerPos));
                G.posGrid[0, y, x].ColorValue = 
                    Color.FromArgb((int)(255 * lumValue), (int)(255 * lumValue), (int)(255 * lumValue));
            } 

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