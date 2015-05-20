
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//------------
using System.Xml;
using System.Xml.Linq;          // XElement


namespace BotUAC
{
    public class TExtension
    {
        private string extensionId;
        private string extensionName;
        public TExtension()  // конструктор  - нельзя без прараметров для структуры !!!
        {
            extensionId = null;
            extensionName = null;
        }
        public TExtension(string ExtensionId, string ExtensionName)
        {
            extensionId = ExtensionId;
            extensionName = ExtensionName;
        }
        public TExtension(XmlNode nodeMain)     
        {
            foreach (XmlNode nodeChild2 in nodeMain.ChildNodes)
            {
                switch (nodeChild2.Name.ToUpper())
                {
                    case "EXTENSIONID": extensionId = nodeChild2.InnerText; break; //------->
                    case "EXTENSIONNAME": extensionName = nodeChild2.InnerText; break; //------->
                }
            }
        }
        public string ExtensionId { get { return extensionId; } set { extensionId = value; } }
        public string ExtensionName { get { return extensionName; } set { extensionName = value; } }
        public XElement ToXElement()
        {
            XElement xe = new XElement("extension");
            xe.Add(new XElement("extensionId", new XText(this.extensionId)));
            xe.Add(new XElement("extensionName", new XText(this.extensionName)));
            return xe; //==============>
        }

        public TExtension Clone()
        {
            TExtension retExtension = new TExtension(this.ExtensionId, this.ExtensionName);
            return retExtension; //==============>
        }

        public override string ToString()
        {
            return extensionId + ", " + extensionName;
        }

    } // class TExtension
}
