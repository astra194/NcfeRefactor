using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Ncfe.CodeTest.Tests
{
    partial class ConfigurationTests
    {
        public static IEnumerable<object[]> GetIsFailoverModeEnabledParameters()
        {
            yield return new object[] { "true", true };
            yield return new object[] { "True", true };
            yield return new object[] { "TRUE", true };
            yield return new object[] { "false", false };
            yield return new object[] { "fAlSe", false };
        }

        [DataTestMethod, DynamicData(nameof(GetIsFailoverModeEnabledParameters), DynamicDataSourceType.Method)]
        public void IsFailoverModeEnabledTests(string setting, bool expectedValue)
        {
            if (!string.IsNullOrEmpty(setting))
                ConfigurationManager.AppSettings["IsFailoverModeEnabled"] = setting;

            bool value = _sut.IsFailoverModeEnabled;

            Assert.AreEqual(expectedValue, value);
        }

        public static IEnumerable<object[]> GetIsFailoverModeEnabledExceptionParameters()
        {
            yield return new object[] { null };
            yield return new object[] { "" };
            yield return new object[] { "maybe" };
            yield return new object[] { "yes" };
            yield return new object[] { "no" };
            yield return new object[] { "0" };
            yield return new object[] { "1" };
        }

        [DataTestMethod, DynamicData(nameof(GetIsFailoverModeEnabledExceptionParameters), DynamicDataSourceType.Method)]
        [ExpectedException(typeof(ApplicationException))]
        public void IsFailoverModeEnabledExceptionTests(string setting)
        {
            if (!string.IsNullOrEmpty(setting))
                ConfigurationManager.AppSettings["IsFailoverModeEnabled"] = setting;

            bool value = _sut.IsFailoverModeEnabled;

            Assert.Fail("Expected exception was not thrown.");
        }
    }
}
