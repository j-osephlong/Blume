using System;
using Map;
using Frames;
using System.Drawing;

class Player
{
    public int xPos {get; private set;}
    public int yPos {get; private set;}
    public int layer {get; private set;}
    private Grid G;
    private Unit playerUnit;

    private int dX, dY;

    public Player (Grid G, Unit playerUnit)
    {
        this.G = G; //not a clone
        this.playerUnit = playerUnit;
        playerUnit.Flags.Add("no_overwrite");
        playerUnit.Flags.Add("wall");
        playerUnit.AddSprite("boy", FrameTools.ReadImageBlocks("Photos/2fgCROP.png", 8, 20));
        playerUnit.AddSprite("guy", FrameTools.ReadImageBlocks("Photos/2.jpg", 8, 20));
        playerUnit.SetSprite("boy");

        dX = 1;
        dY = 1;

        BG  = FrameTools.ReadImageBlocks("Photos/5.jpg", G.width*2, G.height);
    }

    public void Place (int xPos, int yPos, int layer)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        this.layer = layer;

        G.posGrid[layer, this.yPos, this.xPos] = playerUnit;
    }

    public void Move (int deltaX = 0, int deltaY = 0)
    {
        G.posGrid[this.layer, this.yPos + deltaY, this.xPos + deltaX] = playerUnit;
        G.posGrid[this.layer, this.yPos, this.xPos] = null;

        this.xPos += deltaX;
        this.yPos += deltaY;
    }

    public void PlayBall ()
    {
        /*
            Function to be run each main loop cycle for player.
            */
            
        if (KeyboardInput.IsPressed(ConsoleKey.E))
        {
            dX = 2;
            dY = 2;
        }
        else
        {
            dX = 1;
            dY = 1;
        }
        
        if (KeyboardInput.IsPressed(ConsoleKey.A))
            Move(deltaX:-1*dX);
        if (KeyboardInput.IsPressed(ConsoleKey.D))
            Move(deltaX:1*dX);
        if (KeyboardInput.IsPressed(ConsoleKey.W))
            Move(deltaY:-1*dY);
        if (KeyboardInput.IsPressed(ConsoleKey.S))
            Move(deltaY:1*dY);

        if (KeyboardInput.IsPressed(ConsoleKey.P))
        {
            Unit newUnit = new Unit ('X', Color.Brown);
            newUnit.Flags.Add("wall");
            G.posGrid[0, this.yPos, this.xPos] = newUnit;
        }

        if (KeyboardInput.IsPressed(ConsoleKey.O))
        {
            // Unit newUnit = new Unit ('\u2593', Color.Brown);
            // newUnit.Flags.Add("luminant");
            G.posGrid[0, this.yPos, this.xPos].Flags.Add("luminant");
        }

        if (KeyboardInput.IsPressed(ConsoleKey.I))
        {
            G.posGrid[0, this.yPos, this.xPos].ReadOut();
            Console.ReadLine();
            Renderer.PrintFrame(new Frame(G));
        }
    }

    private int d = 0;
    private Grid BG;

    public void MoveBG (long Now)
    {
        // if (playerUnit.currentSprite == "boy")
        //     playerUnit.SetSprite("guy");
        // else
        //     playerUnit.SetSprite("boy");
        Grid.SetLayer(G, 0, d, 0, BG);
        d++;     
    }

}