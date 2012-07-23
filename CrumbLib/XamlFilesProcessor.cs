using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CrumbLib
{
    public class XamlFilesProcessor
    {

        XmlHelper _xmlHelper = new XmlHelper();

        public List<string> FindFiles(string rootFolder, 
            Action<string, string> showError, 
            IMessageHandler msgHandler)
        {
            var xamls = new List<string>();

            msgHandler.AddMessage("Finding Xamls");

            if (!System.IO.Directory.Exists(rootFolder))
            {
                showError("Invalid Folder",
                    "Can't process.  Pick a root directory for your *copied* project.");
                return xamls;
            }
            
            msgHandler.AddMessage("Looking for Xamls in " + rootFolder);
            
            ScanDir(rootFolder, xamls, msgHandler);
                        
            msgHandler.AddMessage(string.Format("Found {0} xaml files.", xamls.Count));

            return xamls;
        }

        public int ProcessXamls(List<string> xamls, 
            Action<string, string> showError, 
            Action<int> setFilesProcessed, IMessageHandler msgHandler)
        {
            
            msgHandler.AddMessage("Inserting Xaml Tags");

            int count = 0;
            int exceptionCount = 0;
            foreach (var file in xamls)
            {
                try
                {
                    msgHandler.AddMessage("Processing " + file);

                    setFilesProcessed(count++);

                    var xaml = File.ReadAllText(file);

                    var fileName = Path.GetFileName(file);

                    var newXaml = this._xmlHelper.TagXmlElements(xaml,
                        s => msgHandler.AddMessage(s, false),
                        fileName + ": " + XmlHelper.LineNumberFormatTag + " " + file
                        );

                    msgHandler.AddMessage("Writing: " + file);

                    // remove read only:
                    System.IO.File.SetAttributes(file, FileAttributes.Normal);
                    System.IO.File.WriteAllText(file, newXaml);
                }
                catch (Exception ex)
                {
                    exceptionCount++;
                    msgHandler.AddMessage("Exception: " + ex.ToString());
                }

            }

            return exceptionCount;
        }

        void ScanDir(string path, List<string> files, IMessageHandler msgHandler)
        {

            var xamls = System.IO.Directory.GetFiles(path)
                .Where(f => f.EndsWith(".xaml",
                    StringComparison.InvariantCultureIgnoreCase));

            foreach (var file in xamls)
            {
                files.Add(file);
                msgHandler.AddMessage(file);
            }

            var dirs = System.IO.Directory.GetDirectories(path);

            foreach (var dir in dirs)
            {
                ScanDir(dir, files, msgHandler);
            }

        }

    }
}
