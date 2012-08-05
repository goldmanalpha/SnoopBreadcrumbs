using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace CrumbLib
{
    using System.Collections;

    public class XmlHelper
    {

        public const string LineNumberFormatTag = "{LineNo}";

        //"xmlns"

        HashSet<string> _requiresXPrefix = new HashSet<string>() { "Class" };
        string[] _validFirstElement = new[] { "ResourceDictionary", "UserControl", "Window" };

        Queue<XmlNode> TraverseTree(string xml)
        {
            var doc = new XmlDocument();
            var q = new Queue<XmlNode>();

            doc.LoadXml(xml);

            TraverseTree(doc.FirstChild, q);

            return q;
        }

        void TraverseTree(XmlNode node, Queue<XmlNode> q)
        {
            q.Enqueue(node);

            foreach (XmlNode child in node.ChildNodes)
            {
                TraverseTree(child, q);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlSource"></param>
        /// <param name="logger"> </param>
        /// <param name="format">must include "{LineNo}" for Line Number spot</param>
        /// <param name="ignoreElements"> </param>
        /// <returns></returns>
        public string TagXmlElements(string xmlSource, Action<string> logger, string format, IList ignoreElements)
        {

            var prefixXOnceList = new List<string>();

            format = format.Replace(LineNumberFormatTag, "{0}");
            var frameworkElements = new AssemblyHelper()
                .GetFrameworkElements().Select(fe => fe.Name).ToList();

            StringBuilder output = new StringBuilder();
            XmlWriterSettings ws = new XmlWriterSettings();
            XmlWriter writer = null;

            var q = this.TraverseTree(xmlSource);

            bool isFirstElement = true;

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

                                if (isFirstElement)
                                {
                                    isFirstElement = false;

                                    if (!_validFirstElement.Contains(eName))
                                    {
                                        logger(string.Format("Not tagging file with first Element: {0}", eName));
                                        return xmlSource;
                                    }
                                }

                                var hasValue = !reader.IsEmptyElement;
                                var currentNode = q.Dequeue();

                                //nodes may have more elements than reader shows, resync if needed
                                int maxTries = 15;
                                while (currentNode.Name != eName && (currentNode = q.Dequeue()) != null && maxTries-- > 0)
                                {
                                }

                                if (currentNode == null || maxTries == 0)
                                {
                                    throw new InvalidOperationException("Can't sync to xaml.  aborting.");
                                }


                                var attributes = new Dictionary<string, string>();


                                for (int i = 0; i < reader.AttributeCount; ++i)
                                {
                                    reader.MoveToNextAttribute();

                                    attributes.Add(reader.Name, reader.Value);
                                }

                                string xmlns = null;
                                if (attributes.ContainsKey("xmlns"))
                                {
                                    xmlns = attributes["xmlns"];
                                    attributes.Remove("xmlns");
                                }
                                //xmlns:cfx

                                if (eName.Contains(":"))
                                {
                                    var split = eName.Split(':');

                                    var extraNamespace = "xmlns:" + split[0];
                                    if (xmlns == null && attributes.ContainsKey(extraNamespace))
                                    {
                                        xmlns = attributes[extraNamespace];
                                        attributes.Remove(extraNamespace);
                                    }

                                    writer.WriteStartElement(split[0], split[1], xmlns);
                                }
                                else
                                {
                                    writer.WriteStartElement(eName, xmlns);
                                }

                                foreach (var attribute in attributes.OrderBy(pair => pair.Key.StartsWith("xmlns") ? 0 : 1))  //put the xmlns first to get the proper output
                                {
                                    //for (int i = 0; i < reader.AttributeCount; ++i)
                                    //{
                                    //    reader.MoveToNextAttribute();
                                    var name = attribute.Key;

                                    if (this._requiresXPrefix.Contains(name) && !prefixXOnceList.Contains(name))
                                    {
                                        prefixXOnceList.Add(name);
                                        name = "x:" + name;
                                    }


                                    if (name.Contains(":"))
                                    {
                                        var split = name.Split(':');
                                        writer.WriteAttributeString(split[0], split[1], null, attribute.Value);
                                    }
                                    else
                                    {
                                        writer.WriteAttributeString(name, attribute.Value);
                                    }

                                }


                                if (frameworkElements.Contains(eName) && !ignoreElements.Contains(eName))
                                {

                                    bool hasTag = reader.GetAttribute("Tag") != null
                                        || (currentNode.ChildNodes.Cast<XmlNode>())
                                        .Any(n => n.Name == eName + ".Tag");

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
