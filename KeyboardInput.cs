using System;
using System.Collections.Generic; 

static class KeyboardInput
{
    private static List<Tuple<ConsoleKey, ConsoleModifiers, bool>> CurrentKeys = new List<Tuple<ConsoleKey, ConsoleModifiers, bool>> ();
    public static List<ConsoleKey> Toggleables = new List<ConsoleKey> ();

    public static void Read()
    {
        /*
            Function to be run in main loop every cycle to read key presses.
            */
            
        for (int x = 0; x < CurrentKeys.Count; x++)
            if (!Toggleables.Contains(CurrentKeys[x].Item1))
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
                if (Toggleables.Contains(KeyPress.Key))
                    CurrentKeys.Add(Tuple.Create(KeyPress.Key, KeyPress.Modifiers, true));
                else
                    CurrentKeys.Add(Tuple.Create(KeyPress.Key, KeyPress.Modifiers, false));
            if (alreadyLogged && Toggleables.Contains(KeyPress.Key))
                for (int x = 0; x < CurrentKeys.Count; x++)
                    if (Toggleables.Contains(CurrentKeys[x].Item1))
                        CurrentKeys.RemoveAt(x);
        }
    }

    public static bool IsPressed (ConsoleKey Key, ConsoleModifiers Mod = 0)
    {
        foreach (Tuple<ConsoleKey, ConsoleModifiers, bool> KeyPress in CurrentKeys)
            if (KeyPress.Item1 == Key && KeyPress.Item2 == Mod)
                return true;

        return false;
    }

    public static void Print ()
    {
        foreach (var key in CurrentKeys)
        {
            Console.WriteLine(key.Item1 + " _" + key.Item2);
        }
    }
}