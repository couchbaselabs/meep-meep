using NUnit.Framework;

namespace MeepMeep.Testing
{
    [TestFixture]
    public abstract class TestsOfStatic : TestsOf { }

    [TestFixture]
    public abstract class TestsOf<T> : TestsOf where T : class
    {
        protected T SUT { get; set; }
    }

    [TestFixture]
    public abstract class TestsOf
    {
        [OneTimeSetUp]
        public void FixtureInitializer()
        {
            OnFixtureInitialize();
        }

        protected virtual void OnFixtureInitialize()
        {
        }

        [OneTimeTearDown]
        public void FixtureFinalizer()
        {
            OnFixtureFinalize();
        }

        protected virtual void OnFixtureFinalize()
        {
        }

        [SetUp]
        public void TestInitializer()
        {
            Now.ValueFn = () => TestConstants.FixedDateTime;
            OnTestInitialize();
        }

        protected virtual void OnTestInitialize()
        {
        }

        [TearDown]
        public void TestFinalizer()
        {
            OnTestFinalize();
            Now.ValueFn = () => TestConstants.FixedDateTime;
        }

        protected virtual void OnTestFinalize()
        {
        }
    }
}
