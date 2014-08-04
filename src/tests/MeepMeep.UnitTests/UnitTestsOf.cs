using MeepMeep.Testing;
using NUnit.Framework;

namespace MeepMeep.UnitTests
{
    [TestFixture]
    public abstract class UnitTestsOfStatic : TestsOfStatic { }

    [TestFixture]
    public abstract class UnitTestsOf<T> : TestsOf<T> where T : class { }

    [TestFixture]
    public abstract class UnitTestsOf : TestsOf { }
}