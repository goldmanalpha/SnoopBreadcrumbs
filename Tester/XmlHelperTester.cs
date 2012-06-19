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

        [Test]
        public void TestInputOutput()
        {
            var files = System.IO.Directory.GetFiles("InputOutput").Where(f => f.EndsWith(".Input.xml"));
                        var helper = new XmlHelper();


            foreach (var file in files)
            {
                var xmlIn = LoadFile(file);

                var outFile = file.Replace(".Input.xml", ".Output.xml");
                var xmlOut = LoadFile(outFile );

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
