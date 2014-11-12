using System.Linq;
using System.Reflection;

namespace MeepMeep.Extensions
{
    internal static class AssemblyExtensions
    {
        internal static T GetAttribute<T>(this Assembly assembly)
        {
            return (T)assembly.GetCustomAttributes(typeof(T), false).First();
        }
    }
}