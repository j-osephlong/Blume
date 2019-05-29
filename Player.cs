using System;
using Map;

class Player
{
    public int xPos {get; private set;}
    public int yPos {get; private set;}
    public int layer {get; private set;}
    private Grid G;
    private Unit playerUnit;

    public Player (Grid G, Unit playerUnit)
    {
        this.G = G; //not a clone
        this.playerUnit = playerUnit;
        playerUnit.Flags.Add("no_overwrite");
    }

    public void Place (int xPos, int yPos, int layer)
    {
        this.xPos = xPos;
        this.yPos = yPos;
        this.layer = layer;

        G.posGrid[layer, this.yPos, this.xPos] = playerUnit;
    }

    public void Move (int xPos, int yPos)
    {
        G.posGrid[this.layer, yPos, xPos] = playerUnit;
        G.posGrid[this.layer, this.yPos, this.xPos] = null;

        this.xPos = xPos;
        this.yPos = yPos;
    }


}