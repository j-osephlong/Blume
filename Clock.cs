using System;
using System.Diagnostics;

static class Clock
{
    /*
        The clock is use to keep time in intervals for subscribing objects.
        A class may subscribe to the clock interval 1/4 second for example, to be run every 1/4 second.
        */

    private static Stopwatch SW = new Stopwatch();
    private static int Threshold = 10;  
    public static long Now = 0;

    public delegate void TimeIntervalMetHandler (long Now);
    public static event TimeIntervalMetHandler OneSecondElapsed;
    public static event TimeIntervalMetHandler HalfSecondElapsed;
    public static event TimeIntervalMetHandler QuarterSecondElapsed;
    public static event TimeIntervalMetHandler EighthSecondElapsed;

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
        // Console.WriteLine(Now % 1000 == 0); 
        if (Now % 1000 == 0 && OneSecondElapsed != null)
            OneSecondElapsed (Now);
        if (Now % 500 == 0 && HalfSecondElapsed != null)
            HalfSecondElapsed (Now);
        if (Now % 250 < Threshold && QuarterSecondElapsed != null)
            QuarterSecondElapsed (Now);
    }

}