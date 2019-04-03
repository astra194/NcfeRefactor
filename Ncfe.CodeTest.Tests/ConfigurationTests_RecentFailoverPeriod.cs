using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Ncfe.CodeTest.Tests
{
    partial class ConfigurationTests
    {
        public static IEnumerable<object[]> GetRecentFailoverPeriodParameters()
        {
            yield return new object[] { "00:00:05", TimeSpan.FromSeconds(5) };
            yield return new object[] { "00:10:00", TimeSpan.FromMinutes(10) };
            yield return new object[] { "20:00:00", TimeSpan.FromHours(20) };
        }

        [DataTestMethod, DynamicData(nameof(GetRecentFailoverPeriodParameters), DynamicDataSourceType.Method)]
        public void RecentFailoverPeriodTests(string setting, TimeSpan expectedValue)
        {
            if (!string.IsNullOrEmpty(setting))
                ConfigurationManager.AppSettings["RecentFailoverTimeSpan"] = setting;

            TimeSpan value = _sut.RecentFailoverPeriod;

            Assert.AreEqual(expectedValue, value);
        }

        public static IEnumerable<object[]> GetRecentFailoverPeriodExceptionParameters()
        {
            yield return new object[] { null };
            yield return new object[] { "" };
            yield return new object[] { "00,10,00" };
            yield return new object[] { "00:60:00" };
        }

        [DataTestMethod, DynamicData(nameof(GetRecentFailoverPeriodExceptionParameters), DynamicDataSourceType.Method)]
        [ExpectedException(typeof(ApplicationException))]
        public void RecentFailoverPeriodExceptionTests(string setting)
        {
            if (!string.IsNullOrEmpty(setting))
                ConfigurationManager.AppSettings["RecentFailoverTimeSpan"] = setting;

            TimeSpan value = _sut.RecentFailoverPeriod;

            Assert.Fail("Expected exception was not thrown.");
        }
    }
}
