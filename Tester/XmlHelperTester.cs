using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CrumbLib;


namespace Tester
{
    [TestFixture]
    public class XmlHelperTester
    {

        [Test]
        public void CanConstruct()
        {
            var x = new XmlHelper();
        }

        List<string> files;
        [TestFixtureSetUp]

        public void Init()
        {

            files = System.IO.Directory.GetFiles(testDocFolder).ToList();
        }

        XmlHelper helper = new XmlHelper();

        const string testDocFolder = "InputOutput";

        void TestSingle(string testName)
        {
            var filesIn = files.Where(f => f.EndsWith(".Input.xml") && f.StartsWith(testDocFolder  + "\\" + testName));

            if (!filesIn.Any())
            {
                Assert.Fail("No files for test: " + testName);
            }

            foreach (var file in filesIn)
            {
                var xmlIn = LoadFile(file);

                var outFile = file.Replace(".Input.xml", ".Output.xml");
                var xmlOut = LoadFile(outFile);

                var tagged = helper.TagXmlElements(xmlIn, "**value**" + XmlHelper.LineNumberFormatTag + "**end**");

                Assert.AreEqual(xmlOut, tagged, "Tagged xml doesn't match: " + outFile);
            }
        }

        [Test]
        public void ExpandedTag()
        {
            this.TestSingle("ExpandedTag");
        }

        [Test]
        public void Xml1()
        {
            this.TestSingle("Xml1");
        }

        [Test]
        public void MCIgnorable()
        {
            this.TestSingle("MC.Ignorable");
        }

        [Test]
        [Ignore]
        public void XClass()
        { 
            this.TestSingle("XClass");
        }


        [Test]
        [Ignore]
        public void XmlNS()
        {
            this.TestSingle("XmlNS");
        }




        [Test]
        public void TestInputOutput()
        {
            var filesIn = files.Where(f => f.EndsWith(".Input.xml"));
            

            foreach (var file in filesIn)
            {
                var xmlIn = LoadFile(file);

                var outFile = file.Replace(".Input.xml", ".Output.xml");
                var xmlOut = LoadFile(outFile);

                var tagged = helper.TagXmlElements(xmlIn, "**value**" + XmlHelper.LineNumberFormatTag + "**end**");

                Assert.AreEqual(xmlOut, tagged, "Tagged xml doesn't match: " + outFile);
            }
        }

        string LoadFile(string fileName)
        {

            return System.IO.File.ReadAllText(fileName);

        }


    }
}
