using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Collections.Generic; 

using Map;

namespace Input
{
    public static class KeyInput
    {
        private static List<Tuple<ConsoleKey, ConsoleModifiers, bool>> CurrentKeys = new List<Tuple<ConsoleKey, ConsoleModifiers, bool>> ();
        
        public static void Read()
        {
            /*
                Function to be run in main loop every cycle to read key presses.
                */
                
            for (int x = 0; x < CurrentKeys.Count; x++)
                CurrentKeys.RemoveAt(x);

            while (Console.KeyAvailable)
            {
                ConsoleKeyInfo KeyPress = Console.ReadKey(true);
                bool alreadyLogged = false;
                foreach (var key in CurrentKeys)
                {
                    if (key.Item1 == KeyPress.Key && key.Item2 == KeyPress.Modifiers)
                        alreadyLogged = true;
                }
                if (!alreadyLogged)
                    CurrentKeys.Add(Tuple.Create(KeyPress.Key, KeyPress.Modifiers, false));
            }
        }

        public static bool IsPressed (ConsoleKey Key, ConsoleModifiers Mod = 0)
        {
            foreach (Tuple<ConsoleKey, ConsoleModifiers, bool> KeyPress in CurrentKeys)
                if (KeyPress.Item1 == Key && KeyPress.Item2 == Mod)
                    return true;

            return false;
        }
    }

    public static class MouseInput
    {
        public static Coord GetPos()
        {
            POINT p;

            GetCursorPos(out p);
            return new Coord (p.X, p.Y);
        }

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
    }
}