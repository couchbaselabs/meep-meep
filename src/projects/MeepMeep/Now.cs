using System;

namespace MeepMeep
{
    public static class Now
    {
        public static Func<DateTime> ValueFn;

        public static DateTime Value
        {
            get { return ValueFn.Invoke(); }
        }

        static Now()
        {
            Reset();
        }

        public static void Reset()
        {
            ValueFn = () => DateTime.Now;
        }
    }
}
