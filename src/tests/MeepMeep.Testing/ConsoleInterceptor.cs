using System;
using System.IO;
using System.Text;

namespace MeepMeep.Testing
{
    public static class ConsoleInterceptor
    {
        private static readonly object Lock = new object();

        public static string Intercept(Action action)
        {
            lock (Lock)
            {
                var intercepted = new StringBuilder();
                var orgWriter = Console.Out;

                using (var writer = new StringWriter(intercepted))
                {
                    try
                    {
                        Console.SetOut(writer);
                        action();
                    }
                    finally
                    {
                        Console.SetOut(orgWriter);
                    }
                }

                return intercepted.ToString().Replace("\r\n", "\n");
            }
        }
    }
}