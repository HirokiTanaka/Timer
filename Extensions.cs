using System;

namespace Timer
{
    internal static class Extensions
    {
        public static string ToTimeSpanString(this TimeSpan ts)
        {
            var ret = string.Empty;
            if (0 < ts.Hours)
            {
                ret = ts.ToString("h'h 'm'm 's's'");
            }
            else if (0 < ts.Minutes)
            {
                ret = ts.ToString("m'm 's's'");
            }
            else
            {
                ret = ts.ToString("s's'");
            }
            return ret;
        }
    }
}
