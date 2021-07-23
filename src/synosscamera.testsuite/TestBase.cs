using System;
using System.Globalization;

namespace synosscamera.testsuite
{
    /// <summary>
    /// Base class for xUnit tests. Provides methods for <see cref="TestInitialize"/> and <see cref="TestCleanUp"/>.
    /// </summary>
    public abstract class TestBase : IDisposable
    {
        /// <summary>
        /// Implement this to initialize your test
        /// </summary>
        protected abstract void TestInitialize();
        /// <summary>
        /// Implement this to clean-up your test
        /// </summary>
        protected abstract void TestCleanUp();
        /// <summary>
        /// Sets the test default thread cultute 
        /// </summary>
        /// <param name="cultureCode">Culture code (defaults to en-US)</param>
        protected virtual void SetTestCulture(string cultureCode = "en-US")
        {
            if (string.IsNullOrEmpty(cultureCode))
                throw new ArgumentNullException(nameof(cultureCode));

            CultureInfo.CurrentCulture = new CultureInfo("en-US");
            CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        }
        /// <summary>
        /// Constructor of the class
        /// </summary>
        protected TestBase()
        {
            SetTestCulture();
            TestInitialize();
        }
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            TestCleanUp();
        }
    }
}
