using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace CrumbLib
{
    public class XmlHelper
    {

        public const string LineNumberFormatTag = "{LineNo}";
        HashSet<string> _requiresXPrefix = new HashSet<string>() { "xmlns", "Class" };

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlSource"></param>
        /// <param name="format">must include "{LineNo}" for Line Number spot</param>
        /// <returns></returns>
        public string TagXmlElements(string xmlSource, string format = LineNumberFormatTag)
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
                ws.ConformanceLevel = ConformanceLevel.Auto;

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
                                var hasValue = !reader.IsEmptyElement;

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

                                    if (this._requiresXPrefix.Contains(name))
                                        name = "x:" + name;

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


                                if (frameworkElements.Contains(eName))
                                {
                                    bool hasTag = reader.GetAttribute("Tag") != null;

                                    if (!hasTag)
                                        writer.WriteAttributeString("Tag",
                                            string.Format(format, reader.LineNumber.ToString()));
                                }

                                if (!hasValue)
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

    }
}
