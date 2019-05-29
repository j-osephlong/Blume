using System;
using System.Collections.Generic; 

static class KeyboardInput
{
    private static List<Tuple<char, ConsoleModifiers, bool>> CurrentKeys = new List<Tuple<char, ConsoleModifiers, bool>> ();
    private static List<char> Toggleables = new List<char> ();

    public static void Read()
    {
        for (int x = 0; x < CurrentKeys.Count; x++)
            if (Toggleables.Contains(CurrentKeys[x].Item1))
                CurrentKeys.RemoveAt(x);

        while (Console.KeyAvailable)
        {
            ConsoleKeyInfo KeyPress = Console.ReadKey(true);
            bool alreadyLogged = false;
            foreach (var key in CurrentKeys)
            {
                if (key.Item1 == KeyPress.KeyChar && key.Item2 == KeyPress.Modifiers)
                    alreadyLogged = true;
            }
            if (!alreadyLogged)
                if (Toggleables.Contains(KeyPress.KeyChar))
                    CurrentKeys.Add(Tuple.Create(KeyPress.KeyChar, KeyPress.Modifiers, true));
                else
                    CurrentKeys.Add(Tuple.Create(KeyPress.KeyChar, KeyPress.Modifiers, false));
        }
    }

    public static void Print ()
    {
        foreach (var key in CurrentKeys)
        {
            Console.WriteLine(key.Item1 + " _" + key.Item2);
        }
    }
}