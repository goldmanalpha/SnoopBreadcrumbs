using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Windows;

namespace SnoopBreadcrumbs
{
    class MainVM : ViewModels.ViewModelBase
    {

        public MainVM()
        {

            DisplayText = "Select the root directory to apply breadcrumbs.\r\n***WARNING.  Xaml files will be overwritten, so using a copy of your project/solution is strongly recommended."; 
        }




        string _displayText;

        public string DisplayText
        {
            get { return _displayText; }
            set
            {
                CheckPropertyChanged("DisplayText", ref _displayText, ref value);
            }
        }



        int _totalFilesToProcess;

        public int TotalFilesToProcess
        {
            get { return _totalFilesToProcess; }
            set
            {
                CheckPropertyChanged("TotalFilesToProcess", ref _totalFilesToProcess, ref value);
            }
        }




        int _filesProcessed;

        public int FilesProcessed
        {
            get { return _filesProcessed; }
            set
            {
                CheckPropertyChanged("FilesProcessed", ref _filesProcessed, ref value);
            }
        }



        string _rootFolder;

        public string RootFolder
        {
            get { return _rootFolder; }
            set
            {
                CheckPropertyChanged("RootFolder", ref _rootFolder, ref value);
            }
        }



        public void TagXmlElements()
        {
            String xmlString =
                    @"<Items > <!-- comment -->
<Item>test with a child element <more/> stuff</Item>
<TextBox /><TextBox Tag=""aTag"" /><ListBox />
</Items>";

            this.DisplayText = TagXmlElements(xmlString, "file pointer ");
        }

        private string TagXmlElements(string xmlSource, string prefix)
        {

            var frameworkElements = new AssemblyHelper()
                .GetFrameworkElements().Select(fe => fe.Name);

            StringBuilder output = new StringBuilder();
            XmlWriterSettings ws = new XmlWriterSettings();
            XmlWriter writer = null;

            // Create an XmlReader
            using (XmlTextReader reader =
                new XmlTextReader(new StringReader(xmlSource)))
            {
                //ws.OmitXmlDeclaration = true;
                //ws.Indent = true;
                //ws.NewLineOnAttributes = true;

                using (writer = XmlWriter.Create(output, ws))
                {
                    // Parse the file and display each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:

                                writer.WriteStartElement(reader.Name);
                                writer.WriteAttributes(reader, false);

                                if (frameworkElements.Contains(reader.Name))
                                {
                                    bool hasTag = reader.GetAttribute("Tag") != null;

                                    if (!hasTag)
                                        writer.WriteAttributeString("Tag",
                                            prefix + " Ln " + reader.LineNumber.ToString());
                                }

                                if (reader.IsEmptyElement)
                                {
                                    writer.WriteEndElement();
                                }
                                break;
                            case XmlNodeType.Text:
                                writer.WriteString(reader.Value);
                                break;
                            case XmlNodeType.XmlDeclaration:
                            case XmlNodeType.ProcessingInstruction:
                                writer.WriteProcessingInstruction(reader.Name, reader.Value);
                                break;
                            case XmlNodeType.Comment:
                                writer.WriteComment(reader.Value);
                                break;
                            case XmlNodeType.EndElement:

                                writer.WriteFullEndElement();
                                break;
                        }
                    }

                }
            }


            XDocument doc = XDocument.Parse(output.ToString());
            return doc.ToString();
        }


        public void ProcessXamls()
        {
            AddMessage("Finding Xamls");


            var root = this.RootFolder;
            if (!System.IO.Directory.Exists(root))
            {
                var msg = "Can't process.  Pick a root directory for your *copied* project.";
                AddMessage(msg);
                MessageBox.Show(msg, "Invalid Folder");
                return;
            }

            AddMessage("Looking for Xamls in " + root);
            
            List<string> xamls = new List<string>();

            ScanDir(root, xamls);

            AddMessage(string.Format("Found {0} xaml files.", xamls.Count));

        }

        void ScanDir(string path, List<string> files)
        {

            var xamls = System.IO.Directory.GetFiles(path)
                .Where(f => f.EndsWith(".xaml",
                    StringComparison.InvariantCultureIgnoreCase));

            foreach (var file in xamls)
            {
                files.Add(file);
                AddMessage(file);
            }

            var dirs = System.IO.Directory.GetDirectories(path);

            foreach (var dir in dirs)
            {
                ScanDir(dir, files);
            }

        }


        void AddMessage(string message, bool isStatus = true, bool isDetail = true)
        {
            var hasText = DisplayText.Length > 0;

            var prefix = string.Empty;

            if (hasText)
                prefix = "\r\n";


            DisplayText += string.Format("{0}  {1} {2}", prefix, DateTime.Now.ToString("hh:mm:ss.fff"), message);
        }
    }

}
