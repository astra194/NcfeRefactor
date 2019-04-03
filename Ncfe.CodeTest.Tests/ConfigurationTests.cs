using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ncfe.CodeTest.Configuration;
using System.Configuration;

namespace Ncfe.CodeTest.Tests
{
    [TestClass]
    public partial class ConfigurationTests
    {
        private IConfiguration _sut;

        [TestInitialize]
        public void Setup()
        {
            _sut = new Configuration.Configuration();


            //ConfigurationManager.AppSettings.Clear();
            foreach (string key in ConfigurationManager.AppSettings.AllKeys)
            {
                ConfigurationManager.AppSettings[key] = null;
            }
        }

        [TestCleanup]
        public void Teardown()
        {
            _sut = null;
        }
    }
}
