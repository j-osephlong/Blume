using System;
using System.Runtime.InteropServices;
using System.Drawing;

static class MouseInput
{
    public static POINT ActualPos;
    public static POINT RelativePos; 
    static bool Init = true;
    
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public static implicit operator Point(POINT point)
        {
            return new Point(point.X, point.Y);
        }
    }

    [DllImport("user32.dll")]
    public static extern bool GetCursorPos(out POINT lpPoint);

    public static void Read()
    {
        KeyboardInput.Read();
        if (KeyboardInput.IsPressed(ConsoleKey.Enter) || Init)
            GetCursorPos(out RelativePos);
        Init = false;
        GetCursorPos(out ActualPos);
        ActualPos.X -= RelativePos.X;
        ActualPos.Y -= RelativePos.Y;

        Console.WriteLine(ActualPos.X + " - " + ActualPos.Y);
    }


}