using System;
using System.Diagnostics;

static class Clock
{
    private static Stopwatch SW = new Stopwatch();
    private static int Threshold = 10;  
    public static long Now = 0;

    public delegate void TimeIntervalMetHandler (long Now);
    public static event TimeIntervalMetHandler OneSecondElapsed;
    public static event TimeIntervalMetHandler HalfSecondElapsed;
    public static event TimeIntervalMetHandler QuarterSecondElapsed;

    public static void Start ()
    {
        SW.Start();
    }

    public static void Tick ()
    {
        if (SW.ElapsedMilliseconds != Now)
            Now = SW.ElapsedMilliseconds;
        else
            return;
            
        if (Now % 1000 == 0 && OneSecondElapsed != null)
        {
            OneSecondElapsed (Now);
        }
        else if (Now % 500 == 0 && HalfSecondElapsed != null)
            HalfSecondElapsed (Now);
        else if (Now % 250 == 0 && QuarterSecondElapsed != null)
        {
            QuarterSecondElapsed (Now);
        }
    }

}