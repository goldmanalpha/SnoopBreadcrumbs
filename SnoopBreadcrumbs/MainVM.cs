using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;

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

String _xmlString =
        @"<?xml version='1.0'?>
<!-- This is a sample XML document -->
<Items>          <Item>test with a child element <more/> stuff</Item>
</Items>";

        public void TestMethod1()
        {
            var doc = new XmlDocument();
            doc.PreserveWhitespace = true;
            doc.Load(new StringReader(_xmlString));

            var builder = new StringBuilder();
            var sw = new StringWriter(builder);

            var tw = new XmlTextWriter(sw);
            try
            {
                tw.Formatting = Formatting.Indented; //this preserves indentation
                doc.Save(tw);
                DisplayText = builder.ToString();
            }
            finally
            {
                tw.Close();
            }



        }


        public void TestMethod()
        {
            StringBuilder output = new StringBuilder();

            // Create an XmlReader
            using (XmlTextReader reader =
                new XmlTextReader(new StringReader(_xmlString)))
            {
                XmlWriterSettings ws = new XmlWriterSettings();
                ws.Indent = true;
                using (XmlWriter writer = XmlWriter.Create(output, ws))
                {

                    // Parse the file and display each of the nodes.
                    while (reader.Read())
                    {
                        switch (reader.NodeType)
                        {
                            case XmlNodeType.Element:
                                writer.WriteStartElement(reader.Name);

                                writer.WriteAttributeString("LineNo", reader.LineNumber.ToString());
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

            this.DisplayText = output.ToString();
        }
    }
}
