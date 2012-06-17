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

            //var x = new System.Xml.XmlTextReader(@"C:\Data\Dropbox\Prog\SnoopBreadcrumbs\SnoopBreadcrumbs\MainVM.cs");

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
                    bool passedFirst = false;

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

                        passedFirst = true;
                    }

                }
            }


            XDocument doc = XDocument.Parse(output.ToString());
            return doc.ToString();
        }
    }
}
