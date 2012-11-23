using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CrumbLib;


namespace Tester
{
    [TestFixture]
    public class XmlHelperXamlTester : FileInputOutputTester
    {
        XmlHelper helper = new XmlHelper();


        protected override string testDocFolder()
        {
            return "InputOutput";
        }

        protected override string FileProcessingFunction(string contents)
        {
            return helper.TagXmlElements(
                contents, 
                s => { }, "**value**" + XmlHelper.LineNumberFormatTag + "**end**", 
                new List<string>());
        }

        [Test]
        public void CanConstruct()
        {
            var x = new XmlHelper();
        }
        
        [Test]
        public void ExpandedTag()
        {
            this.TestSingle("ExpandedTag");
        }

        [Test]
        public void SimpleSetTag()
        {
            this.TestSingle("SimpleSet");
        }

        [Test]
        public void Xml1()
        {
            this.TestSingle("Xml1");
        }

        [Test]
        public void Xml1UnexpectedOrder()
        {
            this.TestSingle("Xml1.UnexpectedOrder");
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
        public void UnexpectedRootTagReturnsUnchanged()
        {
            this.TestSingle("Xml1.UnexpectedRootTag");
        }

        [Test]
        public void MultiNamespaces()
        {
            this.TestSingle("MultiNamespaces");
        }

                /// <summary>
        /// checks all the files to make sure nothing was forgotten
        /// </summary>
        [Test]
        public override void TestAllInputOutput()
        {
            base.TestAllInputOutput();
        }



    }
}
