using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CrumbLib;
using NUnit.Framework;

namespace Tester
{
    [TestFixture]
    public class CommandLineInterpreterTester
    {

        CommandLineInterpreter target;
        
        [SetUp]
        public void Init()
        {
            target = new CommandLineInterpreter();
        }


        [Test]
        public void ParseCommandLine()
        {
            var path = @"C:\data\prog\target" ;

            var result = target.Parse(new[] { path });

            Assert.AreEqual(path, target.Path);
            Assert.AreEqual(0, target.Ignores.Count);

            Assert.IsTrue(result);

        }

        [Test]
        public void ParseIgnores()
        {
            var path = @"C:\data\prog\target";

            var result = target.Parse(new[] { path, "A", "B", "X" });

            Assert.AreEqual(path, target.Path);
            Assert.AreEqual(3, target.Ignores.Count);

            Assert.AreEqual("A", target.Ignores[0]);
            Assert.AreEqual("B", target.Ignores[1]);
            Assert.AreEqual("X", target.Ignores[2]);

            Assert.IsTrue(result);

        }

        [Test]
        public void ParseFailNull()
        {
            var result = target.Parse(null);
            Assert.IsFalse(result);
        }

        [Test]
        public void ParseFailEmpty()
        {
            var result = target.Parse(new string[]{});
            Assert.IsFalse(result);        
        }
    }
}
