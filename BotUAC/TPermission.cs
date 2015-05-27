
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
    public class TPermission
    {
        private string action;
        private string type;
        private TParameters parameters;
        public TPermission()  // конструктор  - нельзя без прараметров для структуры !!!
        {
            action = null;
            type = null;
            parameters = new TParameters();
        }
        public TPermission(string Action, string Type, TParameters Parameters)
        {
            action = Action;
            type = Type;
            parameters = Parameters;
        }
        public TPermission(XmlNode nodeMain)     
        {
            action = null;
            type = null;
            parameters = new TParameters();
            // атрибуты
            action = nodeMain.Attributes["action"].Value;
            type = nodeMain.Attributes["type"].Value;
            // dkj;tyysq vfccbd
            foreach (XmlNode nodeChild2 in nodeMain.ChildNodes)
            {
                switch (nodeChild2.Name.ToUpper())
                {
                    case "PARAMETERS": parameters = new TParameters(nodeChild2); break; //------->
                }
            }
        }
        public string Action { get { return action; } set { action = value; } }
        public string Type { get { return type; } set { type = value; } }
        public TParameters Parameters { get { return parameters; } set { parameters = value; } }
        public XElement ToXElement()
        {
            XElement xe = new XElement("permission", new XAttribute("action", this.action), new XAttribute("type", this.type), this.parameters.ToXElement());
            return xe; //==============>
        }
        public TPermission Clone()
        {
            TPermission retPermission = new TPermission(this.action, this.type, this.parameters.Clone());
            return retPermission; //==============>
        }
        public bool Like(TPermission perm)
        {
            bool bRet = true;
            if (bRet)  // Action
            {
                if (this.action != perm.Action)
                {
                    bRet = false; 
                }
            }
            if (bRet)   // 
            {
                if (this.type != perm.Type)
                {
                    bRet = false; 
                }
            }
            if (bRet)   // parameters
            {
                if (!this.Parameters.Like(this.parameters))
                {
                    bRet = false;
                }
            }
            return bRet; //==============>
        }
        public override string ToString()
        {
            return action + ": " + type + Environment.NewLine + parameters.ToString();
        }

    } // class TPermission
}
