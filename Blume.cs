using System;
using System.Windows.Input;
using System.Drawing;
using Map;
using Frames;
using Input;

class Blume 
{   
    public static void MouseTest()
    {
        while (true)
        {
            Console.SetCursorPosition(0, 0);
            Coord pos = MouseInput.GetPos();
            Console.WriteLine(pos.x + " - " + pos.y);
        }
    }

    public static void BallTest()
    {
        int baseLevel = 21; //unit pos
        int unitHeight = 10; //inner-unit pos 
        int y = 0;


        Grid G = new Grid(50, 25, 2);   
        G.SetLayer(0, 
                GridTools.ApproximateImage("test.jpg", 50, 25)
            );

        Frame F = new Frame(G);
        Renderer.Render(F);

        G[1, y, 5] = new Unit('0');
        G[1, y, 5].SetFlag("rY", (object)10000);

        while (y != baseLevel)
        {
            // Console.WriteLine(G[1, y, 5].GetFlag<int>("rY"));
            G[1, y, 5].SetFlag("rY",
                    (int)((double)(G[1, y, 5].GetFlag<int>("rY"))-(4.8*(y+1)))
                );
                
            if (G[1, y, 5].GetFlag<int>("rY") < 1)
            {
                int yDiff = G[1, y, 5].GetFlag<int>("rY");
                yDiff /= -10;
                yDiff++;

                while (yDiff >= 0)
                {
                    G[1, y+1, 5] = G[1, y, 5];
                    G[1, y, 5] = null;
                    y++;
                    F = new Frame(G);
                    Renderer.Update(F);
                    yDiff--;
                }
                G[1, y, 5].SetFlag("rY", (object)10000);
            }
            F = new Frame(G);
            Renderer.Update(F);
        }
    }

    public static void Main (string [] args)
    {
        // BallTest();
        // MouseTest();


        Grid G = new Grid(100, 50, 2);   
        G.SetLayer(0, 
                GridTools.ApproximateImage("test.jpg", 100, 50)
            );

        for (int i = 0; i < G.width; i++)
            G[0, 21, i].SetFlag("solid", true);
        // G.FillLayer(0, new Unit('\u2593', Color.FromName("SlateBlue")));

        // G[0, 5, 5].SetFlag("lum", 0.95);
        // G[0, 5, 5].ColorValue = Color.FromName("Red");
        // G[0, 7, 6].SetFlag("solid", true);
        // G[0, 7, 6].ColorValue = Color.FromName("Blue");

        // G[0, 5, 8].SetFlag("solid", true);
        // G[0, 5, 8].ColorValue = Color.FromName("Blue");

        // G[0, 4, 4].SetFlag("solid", true);
        // G[0, 4, 4].ColorValue = Color.FromName("Blue");

        Coord pos = new Coord(10, 20, 1);

        G[pos] = new Unit('X', Color.FromName("Green"));
        // G[pos].SetFlag("solid", true);
        G[pos].SetFlag("lum", 0.8);

        Frame F = new Frame(G);
        Renderer.Render(F);


        while (true)
        {
            KeyInput.Read();
            Coord newPos = pos;
            if (KeyInput.IsPressed(ConsoleKey.W))
                newPos.y--;
            else if (KeyInput.IsPressed(ConsoleKey.S))
                newPos.y++;
            if (KeyInput.IsPressed(ConsoleKey.A))
                newPos.x--;
            else if (KeyInput.IsPressed(ConsoleKey.D))
                newPos.x++;
            if (KeyInput.IsPressed(ConsoleKey.L))
                G[0, newPos.y, newPos.x].SetFlag("lum", 0.91);

            if (newPos.x != pos.x || newPos.y != pos.y)
            {
                G[newPos] = G[pos];
                G[pos] = null;
                pos = newPos;
            }
            F = new Frame(G);
            Renderer.Update(F);
        }

        
    }
}