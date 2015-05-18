
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//------------
using System.Xml;
using System.Xml.Linq;          // XElement


namespace BotUAC
{
    public class TAction
    {
        private string actionId;
        private string actionName;
        public TAction()  // конструктор  - нельзя без прараметров для структуры !!!
        {
            actionId = null;
            actionName = null;
        }
        public TAction(string ActionId, string ActionName)
        {
            actionId = ActionId;
            actionName = ActionName;
        }
        public TAction(XmlNode nodeMain)
        {
            foreach (XmlNode nodeChild2 in nodeMain.ChildNodes)
            {
                switch (nodeChild2.Name.ToUpper())
                {
                    case "ACTIONID": actionId = nodeChild2.InnerText; break; //------->
                    case "ACTIONNAME": actionName = nodeChild2.InnerText; break; //------->
                }
            }
        }
        public string ActionId { get { return actionId; } set { actionId = value; } }
        public string ActionName { get { return actionName; } set { actionName = value; } }
        public XElement ToXElement()
        {
            XElement xe = new XElement("action");
          //xe.Add(new XElement("actionId", new XText(this.actionId)));   - не задествовано !
            xe.Add(new XElement("actionName", new XText(this.actionName)));
            return xe; //==============>
        }
        public override string ToString()
        {
            return actionId + ", " + actionName;
        }

    } // class TAction
}
