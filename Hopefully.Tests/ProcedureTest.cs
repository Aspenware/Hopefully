using NUnit.Framework;
using Hopefully;
using System;

namespace Hopefully.Tests
{
    [TestFixture]
    public class TestProcedure
    {
        public class UnreliableProcedure
        {
            public int Failures { get; set; }

            public UnreliableProcedure(int failures = 0)
            {
                Failures = failures;
            }

            public void DoWork()
            {
                if (Failures > 0)
                {
                    Failures--;
                    throw new Exception("Procedure failed");
                }
            }

            public string DoWorkAndReturn()
            {
                if (Failures > 0)
                {
                    Failures--;
                    throw new Exception("Procedure failed");
                }
                return "success";
            }
        }

        [Test]
        public void TestSuccessfulProcedure()
        {
            var proc = new UnreliableProcedure(0);
            int failedAttempts;
            Procedure.Retry(() => proc.DoWork(), out failedAttempts, attempts: 5);
            Assert.AreEqual(0, failedAttempts);
        }

        [Test]
        public void TestFailedProcedure()
        {
            var proc = new UnreliableProcedure(10);
            int failedAttempts = -1;
            Assert.Throws(typeof(Exception), () =>
            {
                Procedure.Retry(() => proc.DoWork(), out failedAttempts, attempts: 5);
            });
            Assert.AreEqual(5, failedAttempts);
        }

        [Test]
        public void TestSuccessAfterFailure()
        {
            var proc = new UnreliableProcedure(3);
            int failedAttempts = -1;
            Assert.DoesNotThrow(() =>
            {
                Procedure.Retry(() => proc.DoWork(), out failedAttempts, attempts: 5);
            });
            Assert.AreEqual(3, failedAttempts);
        }

        [Test]
        public void TestSuccessfulProcedureWithReturn()
        {
            var proc = new UnreliableProcedure(0);
            string returned;
            int failedAttempts;
            returned = Procedure.Retry<string>(() => proc.DoWorkAndReturn(), out failedAttempts, attempts: 5);
            Assert.AreEqual(0, failedAttempts);
            Assert.AreEqual("success", returned);
        }

        [Test]
        public void TestFailedProcedureWithReturn()
        {
            var proc = new UnreliableProcedure(10);
            int failedAttempts = -1;
            string returned = null;
            Assert.Throws(typeof(Exception), () =>
            {
                returned = Procedure.Retry<string>(() => proc.DoWorkAndReturn(), out failedAttempts, attempts: 5);
            });
            Assert.AreEqual(5, failedAttempts);
            Assert.IsNull(returned);
        }

        [Test]
        public void TestSuccessAfterFailureWithReturn()
        {
            var proc = new UnreliableProcedure(3);
            int failedAttempts = -1;
            string returned = null;
            Assert.DoesNotThrow(() =>
            {
                returned = Procedure.Retry<string>(() => proc.DoWorkAndReturn(), out failedAttempts, attempts: 5);
            });
            Assert.AreEqual(3, failedAttempts);
            Assert.AreEqual("success", returned);
        }
    }
}

