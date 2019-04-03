using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Configuration;

namespace Ncfe.CodeTest.Tests
{
    partial class ConfigurationTests
    {
        public static IEnumerable<object[]> GetFailoverRecordsCountThresholdParameters()
        {
            yield return new object[] { "0", 0 };
            yield return new object[] { "5", 5 };
            yield return new object[] { "50", 50 };
            yield return new object[] { "500", 500 };
        }

        [DataTestMethod, DynamicData(nameof(GetFailoverRecordsCountThresholdParameters), DynamicDataSourceType.Method)]
        public void FailoverRecordsCountThresholdTests(string setting, int expectedValue)
        {
            if (!string.IsNullOrEmpty(setting))
                ConfigurationManager.AppSettings["FailoverRecordsCountThreshold"] = setting;

            int value = _sut.FailoverRecordsCountThreshold;

            Assert.AreEqual(expectedValue, value);
        }

        public static IEnumerable<object[]> GetFailoverRecordsCountThresholdExceptionParameters()
        {
            yield return new object[] { null };
            yield return new object[] { "" };
            yield return new object[] { "23.4" };
            yield return new object[] { "4,123" };
        }

        [DataTestMethod, DynamicData(nameof(GetFailoverRecordsCountThresholdExceptionParameters), DynamicDataSourceType.Method)]
        [ExpectedException(typeof(ApplicationException))]
        public void FailoverRecordsCountThresholdExceptionTests(string setting)
        {
            if (!string.IsNullOrEmpty(setting))
                ConfigurationManager.AppSettings["FailoverRecordsCountThreshold"] = setting;

            int value = _sut.FailoverRecordsCountThreshold;

            Assert.Fail("Expected exception was not thrown.");
        }
    }
}
