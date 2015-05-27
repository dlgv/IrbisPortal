
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//----------------
using System.Xml;
using System.Xml.Linq;          // XElement

namespace BotUAC
{
    [Serializable]
    public class TParameter
    {
        private string name;
        private string valuePar;   // !!! "value" - служебное для свойства
        public TParameter()  // конструктор  - нельзя без прараметров для структуры !!!
        {
            name = null;
            valuePar = null;
        }
        public TParameter(string Name, string Value)
        {
            name = Name;
            valuePar = Value;
        }
        public TParameter(XmlNode nodeMain)     
        {
            name = nodeMain.Attributes["name"].Value;
            valuePar = nodeMain.Attributes["value"].Value;
        }
        public string Name { get { return name; } set { name = value; } }
        public string Value { get { return valuePar; } set { valuePar = value; } }
        public XElement ToXElement()     
        {
            XElement xe = new XElement("parameter", new XAttribute("name", this.name), new XAttribute("value", this.valuePar));
            return xe; //==============>
        }
        public TParameter Clone()
        {
            TParameter retParameter = new TParameter(this.name, this.Value);
            return retParameter; //==============>
        }
        public bool Like(TParameter param)
        {
            bool bRet = (this.name == param.Name && this.valuePar == param.Value); 
            return bRet; //==============>
        }
        public override string ToString()
        {
            return name + " = " + valuePar;
        }
    } // class TParameter
}
