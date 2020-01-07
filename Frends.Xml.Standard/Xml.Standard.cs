using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Xml.Xsl;
using Newtonsoft.Json;

namespace Frends.Xml.Standard
{
    /// <summary>
    /// See:  https://github.com/FrendsPlatform/Frends.Xml.Standard
    /// </summary>
    public class Xml
    {
        /// <summary>
        /// Query XML with XPath and return a list of results. See: https://github.com/FrendsPlatform/Frends.Xml.Standard
        /// </summary>
        /// <returns>Object { List &lt;object&gt; Data, List&lt;JToken&gt; ToJson(),JToken ToJson(int index) }</returns>
        public static QueryResults XpathQuery([PropertyTab]QueryInput input, [PropertyTab]QueryOptions options)
        {
            XPathExpression expr = null;
            List<string> xmlTagList = new List<string>();

            try
            {
                expr = XPathExpression.Compile(input.XpathQuery); // will fail if xpath not valid
            }
            catch (Exception)
            {
                throw new Exception($"XPath expression is not valid: {input.XpathQuery}");
            }

           
            var document = new XPathDocument(new StringReader(input.Xml));
            var navigator = document.CreateNavigator();
            var xPathNodeIterator = navigator.Select(expr);



            if (xPathNodeIterator.Count == 0 && options.ThrowErrorOnEmptyResults)
            {
                throw new NullReferenceException($"Could not find any nodes with XPath: {input.XpathQuery}");
            }

            while (xPathNodeIterator.MoveNext())
            {
                xmlTagList.Add(xPathNodeIterator?.Current?.OuterXml ?? string.Empty);
            }

            if (xmlTagList.Count > 0)
            {
                return new QueryResults(xmlTagList);
            }
            return new QueryResults(new List<string>());
        }


        /// <summary>
        /// Query XML with XPath and return a single result. See: https://github.com/FrendsPlatform/Frends.Xml.Standard
        /// </summary>
        /// <returns>Object { object Data, JToken ToJson() } </returns>
        public static QuerySingleResults XpathQuerySingle([PropertyTab]QueryInput input, [PropertyTab]QueryOptions options)
        {

            XPathExpression expr = null;
            XPathNavigator xmlTag = null;

            try
            {
                 expr = XPathExpression.Compile(input.XpathQuery); // will fail if xpath not valid
            }
            catch (Exception)
            {
                throw new Exception($"XPath expression is not valid: {input.XpathQuery}");
            }
            
            var document = new XPathDocument(new StringReader(input.Xml));
            var navigator = document.CreateNavigator();
            
            xmlTag = navigator.SelectSingleNode(expr);
          

            if (xmlTag == null && options.ThrowErrorOnEmptyResults)
            {
                throw new NullReferenceException($"Could not find any nodes with XPath: {input.XpathQuery}");
            }


            if (xmlTag != null)
            {
                return new QuerySingleResults(xmlTag.OuterXml);
            }
            return new QuerySingleResults(null);


        }



        /// <summary>
        /// Convert JSON string to XML string. See: https://github.com/FrendsPlatform/Frends.Xml.Standard
        /// </summary>
        /// <returns>string</returns>
        public static string ConvertJsonToXml(JsonToXmlInput input)
        {
            return JsonConvert.DeserializeXmlNode(input.Json, input.XmlRootElementName).OuterXml;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ValidateResult Validate([PropertyTab]ValidationInput input, [PropertyTab]ValidationOptions options)
        {
            var s = input.Xml as string;
            if (s != null)
            {
                var xmlDocument = new XmlDocument();
                xmlDocument.LoadXml(s);
                return ValidateXmlDocument(xmlDocument, input.XsdSchemas, options);
            }

            var document = input.Xml as XmlDocument;
            if (document != null)
            {
                return ValidateXmlDocument(document, input.XsdSchemas, options);
            }

            throw new InvalidDataException("The input data was not recognized as XML. Supported formats are XML string and XMLDocument.");
        }
        /// <summary>
        /// Validate XML against XML Schema Definitions. See: https://github.com/FrendsPlatform/Frends.Xml.Standard
        /// </summary>
        /// <returns>Object { bool IsValid, string Error } </returns>
        private static ValidateResult ValidateXmlDocument(XmlDocument xmlDocument, IEnumerable<string> inputXsdSchemas, ValidationOptions options)
        {
            var validateResult = new ValidateResult() { IsValid = true };
            var schemas = new XmlSchemaSet();

            var settings = new XmlReaderSettings { ValidationType = ValidationType.Schema };
            settings.ValidationFlags |= XmlSchemaValidationFlags.ReportValidationWarnings;

            foreach (var schema in inputXsdSchemas)
            {
                schemas.Add(null, XmlReader.Create(new StringReader(schema), settings));
            }

            XDocument.Load(new XmlNodeReader(xmlDocument)).Validate(schemas, (o, e) =>
            {

                if (options.ThrowOnValidationErrors)
                {
                    throw new XmlSchemaValidationException(e.Message, e.Exception);
                }
                validateResult.IsValid = false;
                validateResult.Error = e.Message;
            });

            return validateResult;
        }
        /// <summary>
        /// Create a XSLT transformation. See: https://github.com/FrendsPlatform/Frends.Xml.Standard
        /// </summary>
        /// <returns>string</returns>
        public static string Transform(TransformInput input)
        {


            var oldDocument = XDocument.Parse(input.Xml);
            var xsltArgumentList = new XsltArgumentList();
            input.XsltParameters?.ToList().ForEach(x => xsltArgumentList.AddParam(x.Name, string.Empty, x.Value));
           

            using (var stringReader = new StringReader(input.Xslt))
            {
                using (var xsltReader = XmlReader.Create(stringReader))
                {
                    // Create the XsltSettings object with script enabled.
                    var settings = new XsltSettings(false, true);
                    var transformer = new XslCompiledTransform();
                    
                    transformer.Load(xsltReader, settings ,new XmlUrlResolver());
                    using (var oldDocumentReader = oldDocument.CreateReader())
                    {

                        using (var newDocument = new MemoryStream()){

                            transformer.Transform(oldDocumentReader, xsltArgumentList, newDocument);

                            newDocument.Position = 0;
                            using (var newDocumentReader = new StreamReader(newDocument, transformer.OutputSettings.Encoding, false))
                            {
                                string output = newDocumentReader.ReadToEnd();
                                return output;
                            }

                        }

                    }
                }
            }

        }


    }



}
