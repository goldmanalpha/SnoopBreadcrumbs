using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Tester
{
    public abstract class FileInputOutputTester
    {


        List<string> _files;
        protected List<string> files { 
            get {

                if (_files == null)
                    _files =  System.IO.Directory
                        .GetFiles(testDocFolder())
                        .ToList();

                return _files.ToList();
        } }

        protected abstract string testDocFolder();
        protected abstract string FileProcessingFunction(string contents);

        protected void TestSingle(string testName)
        {
            var filesIn = files.Where(f => f == testDocFolder() + "\\" + testName + ".Input.xml");

            if (!filesIn.Any())
            {
                Assert.Fail("No files for test: " + testName);
            }

            foreach (var inFile in filesIn)
            {
                var xmlIn = LoadFile(inFile);

                var outFile = inFile.Replace(".Input.xml", ".Output.xml");
                var xmlOut = System.IO.File.Exists(outFile) ? LoadFile(outFile) : null;

                var processed = this.FileProcessingFunction(xmlIn);

                //for outputting expected values:
                //System.IO.File.WriteAllText(outFile, tagged);

                Assert.AreEqual(xmlOut, processed, "Tagged xml doesn't match: " + outFile);

                //in case the source needs a modification                
                //System.IO.File.WriteAllText(inFile, xmlIn);

            }
        }

        /// <summary>
        /// checks all the files to make sure nothing was forgotten
        /// </summary>
        public  virtual void TestAllInputOutput()
        {
            var TestIn = files.Where(f => f.EndsWith(".Input.xml"))
                .Select(f => f.Replace(".Input.xml", ""))
                .Select(f => f.Split(new[] { "\\" }, StringSplitOptions.None).Last());

            foreach (var test in TestIn)
            {
                this.TestSingle(test);
            }
        }


        protected string LoadFile(string fileName)
        {
            return System.IO.File.ReadAllText(fileName);
        }

    }
}
