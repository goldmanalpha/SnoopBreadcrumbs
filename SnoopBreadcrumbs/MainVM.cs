using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System.Xml.Linq;
using System.Windows;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SnoopBreadcrumbs
{
    class MainVM : ViewModels.ViewModelBase
    {

        public MainVM()
        {
            this.RootFolder = @"C:\temp";
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


        const string LineNumberFormatTag = "{LineNo}";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlSource"></param>
        /// <param name="format">must include "{LineNo}" for Line Number spot</param>
        /// <returns></returns>
        private string TagXmlElements(string xmlSource, string format = LineNumberFormatTag)
        {
            format = format.Replace(LineNumberFormatTag, "{0}");
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
                                //writer.WriteAttributes(reader, false);
                                var eName = reader.Name;

                                if (eName.Contains(":"))
                                    {
                                        var split = eName.Split(':');
                                        writer.WriteStartElement(split[0], split[1], null);
                                    }
                                    else
                                    {
                                        writer.WriteStartElement(eName);
                                    }

                                for (int i = 0; i < reader.AttributeCount; ++i)
                                {
                                    reader.MoveToNextAttribute();
                                    var name = reader.Name;

                                    if (name == "xmlns")
                                        name = "x:xmlns";

                                    if (name.Contains(":"))
                                    {
                                        var split = name.Split(':');
                                        writer.WriteAttributeString(split[0], split[1], null, reader.Value);
                                    }
                                    else
                                    {
                                        writer.WriteAttributeString(name, reader.Value);
                                    }
                                    
                                }


                                if (frameworkElements.Contains(reader.Name))
                                {
                                    bool hasTag = reader.GetAttribute("Tag") != null;

                                    if (!hasTag)
                                        writer.WriteAttributeString("Tag",
                                            string.Format(format, reader.LineNumber.ToString()));
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
            var task = Task.Factory.StartNew(ProcessXamls2);

            task.ContinueWith(obj =>
                DisplayText += "\r\nFinished");

        }

        public void ProcessXamls2()
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

            DisplayText = string.Empty;

            AddMessage("Looking for Xamls in " + root);

            List<string> xamls = new List<string>();

            ScanDir(root, xamls);

            AddMessage(string.Format("Found {0} xaml files.", xamls.Count));

            AddMessage("Inserting Xaml Tags");

            int count = 0;
            foreach (var file in xamls)
            {
                AddMessage("Processing " + file);

                this.FilesProcessed = count++;

                var xaml = System.IO.File.ReadAllText(file);

                var fileName = Path.GetFileName(file);

                var newXaml = this.TagXmlElements(xaml,
                    fileName + ": " + LineNumberFormatTag + " " + file);

                //System.IO.File.WriteAllText(file, newXaml);



            }


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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="message"></param>
        /// <param name="isStatus">updates 1 line status</param>
        /// <param name="isDetail">updates running log</param>
        void AddMessage(string message, bool isStatus = true, bool isDetail = true)
        {
            var hasText = DisplayText.Length > 0;

            var prefix = string.Empty;

            if (hasText)
                prefix = "\r\n";

            if (isDetail)
                DisplayText += string.Format("{0}  {1} {2}", 
                    prefix, DateTime.Now.ToString("hh:mm:ss.fff"), message);
        }
    }

}
