/*
    NON FUNCTIONAL
    */

using System;
using System.IO;
using System.Text;
using Map;

static class BlumeIO
{
    public static void WriteGrid (this Grid G)
    {
        using (FileStream fs = File.Create("gridtest.txt"))
        {
            Byte [] gridconfig = new UTF8Encoding(true).GetBytes("w: " + G.width + " h: " + G.height + " d: " + G.depth);
            fs.Write(gridconfig, 0, gridconfig.Length);
            fs.Write(gridconfig);
        }
    }
}