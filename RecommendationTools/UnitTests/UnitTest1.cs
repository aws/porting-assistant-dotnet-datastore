using NUnit.Framework;
using System;

namespace UnitTests
{
    public class Tests
    {

        [Test]
        public void Test1()
        {
            Console.WriteLine("Running test 1");
            Assert.AreEqual(1, 1);
        }
    }
}